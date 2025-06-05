import { useEffect, useState } from 'react';
import api from '../api/axios'; 
import Navbar from '../components/Navbar'; 
import { getUserFromToken } from '../utils/auth'; 
import { useNavigate } from 'react-router-dom'; 
import './DashboardPage.css'; 

function DashboardPage() {
  const [stats, setStats] = useState({
    projectCount: 0,
    userCount: 0,
    customerCount: 0,
    todos: [],
  });

  const user = getUserFromToken(); // Giriş yapan kullanıcı
  const navigate = useNavigate();  // navigate tanımlandı

  useEffect(() => {
    // Giriş yapılmamışsa login'e yönlendir
    if (!user) {
      navigate('/login');
      return;
    }

    // Her kullanıcı için bu istekler yapılır
    const requests = [
      api.get('/project'),
      api.get('/todo')
    ];

    // Sadece Admin kullanıcılar ek verileri alabilir
    if (user.role === "Admin") {
      requests.push(api.get('/user'));
      requests.push(api.get('/customer'));
    }

    Promise.all(requests)
      .then(responses => {
        // Cevapları sırayla al
        const projects = responses[0].data;
        const todos = responses[1].data;

        let users = [];
        let customers = [];

        if (user.role === "Admin") {
          users = responses[2]?.data || [];
          customers = responses[3]?.data || [];
        }

        setStats({
          projectCount: projects.length,
          userCount: users.length,
          customerCount: customers.length,
          todos: todos,
        });
      })
      .catch(err => {
        console.error("Dashboard verileri alınırken hata oluştu:", err);
      });
  }, [user, navigate]);

  const completedTodos = stats.todos.filter(t => t.status === "Tamamlandı").length;
  const pendingTodos = stats.todos.filter(t => t.status !== "Tamamlandı").length;

  return (
    <>
      <Navbar />
      <div className="dashboard-container">
        <h2>📊 Yönetim Paneli {user?.name && `— Hoşgeldin, ${user.name}!`}</h2>

        <div className="dashboard-grid">
          <div className="dashboard-card">
            <h3>📁 Projeler</h3>
            <p>{stats.projectCount}</p>
          </div>

          {/* Sadece Admin'e göster */}
          {user?.role === "Admin" && (
            <>
              <div className="dashboard-card">
                <h3>👤 Kullanıcılar</h3>
                <p>{stats.userCount}</p>
              </div>
              <div className="dashboard-card">
                <h3>🏢 Müşteriler</h3>
                <p>{stats.customerCount}</p>
              </div>
            </>
          )}

          <div className="dashboard-card">
            <h3>✅ Tamamlanan Görev</h3>
            <p>{completedTodos}</p>
          </div>
          <div className="dashboard-card">
            <h3>⏳ Bekleyen Görev</h3>
            <p>{pendingTodos}</p>
          </div>
        </div>
      </div>
    </>
  );
}

export default DashboardPage;

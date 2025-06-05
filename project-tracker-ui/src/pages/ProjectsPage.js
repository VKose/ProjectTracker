import Navbar from '../components/Navbar';
import { useEffect, useState } from 'react';
import api from '../api/axios'; 
import { useNavigate } from 'react-router-dom'; 
import ProjectCard from '../components/ProjectCard';
import './ProjectsPage.css'; 
import { getUserFromToken } from '../utils/auth'; 

function ProjectsPage() {
  // Proje, kullanıcı, müşteri ve hata verileri için state'ler tanımlanıyor
  const [projects, setProjects] = useState([]);
  const [error, setError] = useState('');
  const [users, setUsers] = useState([]);
  const [customers, setCustomers] = useState([]);
  const navigate = useNavigate();
  const user = getUserFromToken(); // Token'dan kullanıcı bilgisi alınır

  // Sayfa yüklendiğinde projeler, kullanıcılar ve müşteriler API'den çekilir
  useEffect(() => {
    // Giriş yapılmamışsa yönlendir
    if (!user) {
      navigate('/login');
      return;
    }

    // Proje verilerini al
    api.get('/project')
      .then(res => setProjects(res.data))
      .catch(err => {
        // Eğer yetkisizse giriş sayfasına yönlendir
        if (err.response?.status === 401) navigate('/login');
        else setError("Projeler alınamadı."); // Diğer hatalarda mesaj göster
      });

    api.get('/user/assignable').then(res => setUsers(res.data)).catch(() => {});  // Kullanıcı verilerini al 
    api.get('/customer').then(res => setCustomers(res.data)).catch(() => {});     // Müşteri verilerini al
  }, [navigate, user]); 

  // Kullanıcı atama fonksiyonu
  const assignUser = async (projectId, userId) => {
    try {
      await api.post(`/project/${projectId}/assign-user/${userId}`);
      // Güncel projeleri tekrar al
      const res = await api.get('/project');
      setProjects(res.data);
    } catch {
      alert("Kullanıcı atanamadı.");
    }
  };

  // Müşteri atama fonksiyonu
  const assignCustomer = async (projectId, customerId) => {
    try {
      await api.post(`/project/${projectId}/assign-customer/${customerId}`);
      // Güncel projeleri tekrar al
      const res = await api.get('/project');
      setProjects(res.data);
    } catch {
      alert("Müşteri atanamadı.");
    }
  };

  return (
    <>
      <Navbar />
      <div className="projects-container">
        <h2>📁 Proje Listesi</h2>

        {/* Hata varsa mesaj göster */}
        {error && <p className="error-message">{error}</p>}

        {/* Projeleri kart şeklinde gösteren alan */}
        <div className="project-grid">
          {projects.map(p => (
            <ProjectCard
              key={p.id}
              project={p}
              users={users}
              customers={customers}
              onAssignUser={assignUser}
              onAssignCustomer={assignCustomer}
            />
          ))}
        </div>
      </div>
    </>
  );
}

export default ProjectsPage;

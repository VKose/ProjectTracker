import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../api/axios';
import Navbar from '../components/Navbar';
import { getUserFromToken } from '../utils/auth'; 
import './TodoDetailPage.css'; 

function TodoDetailPage() {
  const { id } = useParams();             // URL'den görev ID'sini al
  const navigate = useNavigate();   
  const user = getUserFromToken();  

  const [todo, setTodo] = useState(null); // Görev bilgisi
  const [users, setUsers] = useState([]); // Kullanıcı listesi
  const [error, setError] = useState(''); // Hata mesajı

  const statusOptions = ["Bekliyor", "Devam Ediyor", "Tamamlandı"]; // Görev durumları

  // Sayfa yüklendiğinde görev ve kullanıcılar alınır
  useEffect(() => {
    // Görev bilgisi al
    api.get(`/todo`)
      .then(res => {
        const found = res.data.find(t => t.id === parseInt(id)); // ID ile görev bul
        setTodo(found);
      })
      .catch(() => setError("Görev alınamadı."));

    // Kullanıcı listesi al
    api.get('/user/assignable')
      .then(res => setUsers(res.data))
      .catch(() => {}); // Hata durumunda sessiz geç
  }, [id]);

  // Durum güncelleme
  const handleStatusChange = async (newStatus) => {
    await api.put(`/todo/${id}/status`, JSON.stringify(newStatus), {
      headers: { 'Content-Type': 'application/json' }
    });
    // Güncellenmiş görev verisini tekrar çek
    const updated = await api.get('/todo');
    const found = updated.data.find(t => t.id === parseInt(id));
    setTodo(found);
  };

  // Kullanıcı atama
  const handleUserAssign = async (userId) => {
    await api.post(`/todo/${id}/assign-user/${userId}`);
    const updated = await api.get('/todo');
    const found = updated.data.find(t => t.id === parseInt(id));
    setTodo(found);
  };

  // E-posta gönder
  const handleNotify = async () => {
    try {
      await api.post(`/todo/${id}/notify`);
      alert("📧 Görev sahibine e-posta gönderildi!");
    } catch {
      alert("E-posta gönderilemedi.");
    }
  };

  // Görevi sil
  const handleDelete = async () => {
    const confirmDelete = window.confirm("Bu görevi silmek istediğinize emin misiniz?");
    if (!confirmDelete) return;

    try {
      await api.delete(`/todo/${id}`);
      alert("Görev silindi.");
      navigate('/todos'); // Listeye dön
    } catch {
      alert("Görev silinemedi.");
    }
  };

  // Yüklenme veya hata durumu
  if (!todo) return <p className="todo-detail-loading">{error || 'Yükleniyor...'}</p>;

  return (
    <>
      <Navbar />
      <div className="todo-detail-container">
        <h2>📄 Görev Detayı</h2>

        {/* Görev bilgileri */}
        <div className="todo-info-box">
          <p><strong>📌 Başlık:</strong> {todo.title}</p>
          <p><strong>📝 Açıklama:</strong> {todo.description || '—'}</p>
          <p><strong>📅 Termin:</strong> {new Date(todo.dueDate).toLocaleDateString()}</p>
          <p><strong>📂 Proje:</strong> {todo.project?.title ?? '—'}</p>
          <p><strong>👤 Sorumlu:</strong> {todo.assignedUser?.name ?? '—'}</p>
          <p><strong>📌 Durum:</strong> {todo.status}</p>
        </div>

        {/* Yalnızca Admin ve Manager'lar için aksiyonlar */}
        {(user?.role === 'Admin' || user?.role === 'Manager') && (
          <div className="todo-actions">
            <label>📌 Durumu Güncelle:</label>
            <select value={todo.status} onChange={e => handleStatusChange(e.target.value)}>
              {statusOptions.map(opt => <option key={opt} value={opt}>{opt}</option>)}
            </select>

            <label>👥 Kullanıcı Ata:</label>
            <select value={todo.assignedUserId || ''} onChange={e => handleUserAssign(e.target.value)}>
              <option value="">— Seç —</option>
              {users.map(u => (
                <option key={u.id} value={u.id}>{u.name} ({u.role})</option>
              ))}
            </select>

            <button onClick={handleNotify}>📩 Mail Gönder</button>
          </div>
        )}

        {/* Sadece Admin silme yetkisine sahiptir */}
        {user?.role === 'Admin' && (
          <div className="todo-delete">
            <button onClick={handleDelete}>🗑️ Görevi Sil</button>
          </div>
        )}

        {/* Listeye dönüş butonu */}
        <div className="todo-back">
          <button onClick={() => navigate('/todos')}>← Görev Listesine Dön</button>
        </div>
      </div>
    </>
  );
}

export default TodoDetailPage;

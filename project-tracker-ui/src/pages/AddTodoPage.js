import Navbar from '../components/Navbar';
import { useState, useEffect } from 'react';
import api from '../api/axios';
import { useNavigate } from 'react-router-dom';
import './AddTodoPage.css';
import { getUserFromToken } from '../utils/auth'; 

function AddTodoPage() {
  const navigate = useNavigate();
  const user = getUserFromToken(); 

  // Form alanları için state tanımlamaları
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [dueDate, setDueDate] = useState('');
  const [projectId, setProjectId] = useState('');
  const [assignedUserId, setAssignedUserId] = useState('');

  // Liste verileri (projeler ve kullanıcılar)
  const [projects, setProjects] = useState([]);
  const [users, setUsers] = useState([]);

  // Hata mesajı göstermek için state
  const [msg, setMsg] = useState('');

  // Sayfa yüklendiğinde projeler ve kullanıcılar çekilir
  useEffect(() => {
    // Giriş kontrolü ve yetki kontrolü
    if (!user || (user.role !== 'Admin' && user.role !== 'Manager')) {
      navigate('/login');
      return;
    }

    api.get('/project')
      .then(res => setProjects(res.data)) // Projeler başarıyla alındı
      .catch(() => setMsg("❗ Proje listesi alınamadı."));

    api.get('/user/assignable')
      .then(res => setUsers(res.data)) // Kullanıcılar başarıyla alındı
      .catch(() => setMsg("❗ Kullanıcı listesi alınamadı."));
  }, [navigate, user]);

  // Form gönderildiğinde çalışacak fonksiyon
  const handleSubmit = async (e) => {
    e.preventDefault(); // Sayfa yenilenmesini engeller

    try {
      // Görev API'ye gönderilir
      await api.post('/todo', {
        title,
        description,
        dueDate,
        projectId: parseInt(projectId),
        // Kullanıcı atanmadıysa, görevi giriş yapan kullanıcıya ata
        assignedUserId: assignedUserId
          ? parseInt(assignedUserId)
          : parseInt(user?.id)
      });

      // Başarılıysa görev listesi sayfasına yönlendir
      navigate('/todos');
    } catch {
      // Hata durumunda kullanıcıya mesaj göster
      setMsg("❌ Görev eklenemedi. Lütfen bilgileri kontrol et.");
    }
  };

  return (
    <>
      <Navbar />
      <div className="add-todo-container">
        <h2>📝 Yeni Görev Ekle</h2>

        {/* Hata veya bilgi mesajı gösterimi */}
        {msg && <p className="error-message">{msg}</p>}

        {/* Görev ekleme formu */}
        <form onSubmit={handleSubmit} className="todo-form">
          <label>Başlık:</label>
          <input
            type="text"
            value={title}
            onChange={e => setTitle(e.target.value)}
            required
          />

          <label>Açıklama:</label>
          <textarea
            value={description}
            onChange={e => setDescription(e.target.value)}
            rows={4}
          />

          <label>Son Teslim Tarihi:</label>
          <input
            type="date"
            value={dueDate}
            onChange={e => setDueDate(e.target.value)}
            required
          />

          <label>Proje Seç:</label>
          <select
            value={projectId}
            onChange={e => setProjectId(e.target.value)}
            required
          >
            <option value="">-- Proje Seç --</option>
            {projects.map(p => (
              <option key={p.id} value={p.id}>{p.title}</option>
            ))}
          </select>

          <label>Sorumlu Kullanıcı:</label>
          <select
            value={assignedUserId}
            onChange={e => setAssignedUserId(e.target.value)}
          >
            <option value="">-- Kullanıcı Seç (opsiyonel) --</option>
            {users.map(u => (
              <option key={u.id} value={u.id}>{u.name} ({u.role})</option>
            ))}
          </select>

          <button type="submit">✅ Kaydet</button>
        </form>
      </div>
    </>
  );
}

export default AddTodoPage;

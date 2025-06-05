import Navbar from '../components/Navbar';
import { useState, useEffect } from 'react';
import api from '../api/axios';
import { useNavigate } from 'react-router-dom';
import './AddProjectPage.css';
import { getUserFromToken } from '../utils/auth'; // Token'dan kullanıcı bilgisi çek

function AddProjectPage() {
  const navigate = useNavigate();

  // Form inputları için state'ler
  const [title, setTitle] = useState('');               // Proje başlığı
  const [description, setDescription] = useState('');   // Proje açıklaması
  const [status, setStatus] = useState('Bekliyor');     // Proje durumu (varsayılan)
  const [customerId, setCustomerId] = useState('');     // Seçilen müşteri ID

  // Listeleme için state'ler
  const [customers, setCustomers] = useState([]);       // Müşteri listesi
  const [msg, setMsg] = useState('');                   // Hata / bilgi mesajı
  const [loading, setLoading] = useState(false);        // Form gönderim durumu

  const user = getUserFromToken(); // Giriş yapan kullanıcı

  // Sayfa yüklendiğinde müşterileri çek
  useEffect(() => {
    // Yetkisiz girişleri engelle
    if (!user || (user.role !== 'Admin' && user.role !== 'Manager')) {
      navigate('/login');
      return;
    }

    api.get('/customer')
      .then(res => setCustomers(res.data))                  // Başarılıysa listeyi kaydet
      .catch(() => setMsg("❗ Müşteri listesi alınamadı.")); // Hata durumunda mesaj göster
  }, [navigate, user]);

  // Form gönderildiğinde çalışacak işlem
  const handleSubmit = async (e) => {
    e.preventDefault();       // Sayfa yenilemeyi engelle
    setLoading(true);         // Yükleniyor durumunu başlat
    setMsg('');               // Önceki mesajı temizle

    try {
      // API'ye yeni proje gönder
      await api.post('/project', {
        title: title.trim(),
        description: description.trim(),
        status,
        customerId: customerId ? parseInt(customerId) : null
      });

      // Başarılıysa proje listesine yönlendir
      navigate('/projects');
    } catch {
      // Hata durumunda mesaj göster
      setMsg("❌ Proje eklenemedi. Lütfen bilgileri kontrol edin.");
    } finally {
      setLoading(false); // Yüklenme durumunu kapat
    }
  };

  return (
    <>
      <Navbar />
      <div className="add-project-container">
        <h2>🆕 Yeni Proje Ekle</h2>

        {/* Mesaj alanı */}
        {msg && <p className="error-message">{msg}</p>}

        {/* Proje ekleme formu */}
        <form onSubmit={handleSubmit} className="project-form">
          {/* Başlık inputu */}
          <label>Başlık:</label>
          <input
            type="text"
            value={title}
            onChange={e => setTitle(e.target.value)}
            required
            placeholder="Proje başlığı"
          />

          {/* Açıklama text alanı */}
          <label>Açıklama:</label>
          <textarea
            value={description}
            onChange={e => setDescription(e.target.value)}
            rows={4}
            placeholder="Açıklama (isteğe bağlı)"
          />

          {/* Durum seçimi */}
          <label>Durum:</label>
          <select value={status} onChange={e => setStatus(e.target.value)}>
            <option value="Bekliyor">Bekliyor</option>
            <option value="Devam Ediyor">Devam Ediyor</option>
            <option value="Tamamlandı">Tamamlandı</option>
          </select>

          {/* Müşteri seçimi */}
          <label>Müşteri:</label>
          <select value={customerId} onChange={e => setCustomerId(e.target.value)}>
            <option value="">-- Müşteri Seç (opsiyonel) --</option>
            {customers.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>

          {/* Kaydet butonu */}
          <button type="submit" disabled={loading}>
            {loading ? 'Kaydediliyor...' : '📁 Projeyi Kaydet'}
          </button>
        </form>
      </div>
    </>
  );
}

export default AddProjectPage;

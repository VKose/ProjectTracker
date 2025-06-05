import { useEffect, useState } from 'react';
import api from '../api/axios';
import Navbar from '../components/Navbar';
import { getUserFromToken } from '../utils/auth';
import { useNavigate } from 'react-router-dom';

function CustomersPage() {
  
  const [customers, setCustomers] = useState([]);                   // Müşteri listesi
  const [form, setForm] = useState({ name: '', contactInfo: '' });  // Form alanları (yeni müşteri ekleme/güncelleme için)
  const [editMode, setEditMode] = useState(false);                  // Güncelleme modu açık mı?
  const [editId, setEditId] = useState(null);                       // Hangi müşteri düzenleniyor(id)?  
  const [message, setMessage] = useState('');                       // Başarı / hata mesajı
  const user = getUserFromToken();                                  // Giriş yapan kullanıcı
  const navigate = useNavigate();                                   // Sayfa yönlendirme

  // Sayfa yüklendiğinde müşterileri çek
  useEffect(() => {
    if (!user) {
      navigate('/login'); // Giriş yapılmamışsa login sayfasına yönlendir
      return;
    }

    api.get('/customer')
      .then(res => setCustomers(res.data))
      .catch(() => setMessage("❌ Müşteriler alınamadı."));
  }, [navigate, user]); 

  // Form input değişikliklerini yakala
  const handleInputChange = (e) => {
    setForm(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  // Müşteri ekleme veya güncelleme
  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      if (editMode) {
        // Güncelleme işlemi
        await api.put(`/customer/${editId}`, form);
        setMessage("✅ Müşteri güncellendi.");
      } else {
        // Yeni müşteri ekleme
        await api.post('/customer', form);
        setMessage("✅ Müşteri eklendi.");
      }

      // Listeyi yeniden çek ve formu sıfırla
      const res = await api.get('/customer');
      setCustomers(res.data);
      setForm({ name: '', contactInfo: '' });
      setEditMode(false);
      setEditId(null);
    } catch {
      setMessage("❌ İşlem başarısız.");
    }
  };

  // Müşteri silme
  const handleDelete = async (id) => {
    if (!window.confirm("Bu müşteri silinsin mi?")) return;

    try {
      await api.delete(`/customer/${id}`);
      setCustomers(prev => prev.filter(c => c.id !== id));
    } catch {
      alert("Silme işlemi başarısız.");
    }
  };

  // Yetki kontrolü
  const isAdmin = user?.role === 'Admin';                               // Admin ise altta silme işlemi için
  const canEdit = user?.role === 'Admin' || user?.role === 'Manager';   // Düzenleyebilen Admin, Manager

  return (
    <>
      <Navbar />

      <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
        <h2>🏢 Müşteri Yönetimi</h2>

        {/* Bilgilendirme mesajı */}
        {message && <p style={{ color: message.startsWith('✅') ? 'green' : 'red' }}>{message}</p>}

        {/* Müşteri Ekleme/Güncelleme Formu */}
        {canEdit && (
          <form onSubmit={handleSubmit} style={{ marginBottom: '30px' }}>
            <h3>{editMode ? '✏️ Müşteri Güncelle' : '➕ Müşteri Ekle'}</h3>

            <input
              type="text"
              name="name"
              placeholder="Müşteri Adı"
              value={form.name}
              onChange={handleInputChange}
              required
              style={{ padding: '8px', width: '100%', marginBottom: '10px' }}
            />

            <input
              type="text"
              name="contactInfo"
              placeholder="İletişim Bilgileri (E-mail)"
              value={form.contactInfo}
              onChange={handleInputChange}
              required
              style={{ padding: '8px', width: '100%', marginBottom: '10px' }}
            />

            <button
              type="submit"
              style={{
                padding: '10px 20px',
                backgroundColor: '#007bff',
                color: 'white',
                border: 'none'
              }}
            >
              {editMode ? 'Güncelle' : 'Ekle'}
            </button>
          </form>
        )}

        {/* Müşteri Listesi Tablosu */}
        <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Ad</th>
              <th>İletişim</th>
              {canEdit && <th>İşlem</th>}
            </tr>
          </thead>
          <tbody>
            {customers.map(c => (
              <tr key={c.id}>
                <td>{c.id}</td>
                <td>{c.name}</td>
                <td>{c.contactInfo}</td>
                {canEdit && (
                  <td>
                    {/* Müşteriyi düzenle */}
                    <button
                      onClick={() => {
                        setForm({ name: c.name, contactInfo: c.contactInfo });
                        setEditId(c.id);
                        setEditMode(true);
                      }}
                      style={{ marginRight: '10px' }}
                    >
                      Düzenle
                    </button>

                    {/* Adminse silme butonu görünür */}
                    {isAdmin && (
                      <button
                        onClick={() => handleDelete(c.id)}
                        style={{ backgroundColor: 'red', color: 'white' }}
                      >
                        Sil
                      </button>
                    )}
                  </td>
                )}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}

export default CustomersPage;

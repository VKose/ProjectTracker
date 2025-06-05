import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios'; 
import Navbar from '../components/Navbar'; 

function RegisterPage() {
  const navigate = useNavigate(); 

  // Form alanlarını tutan state
  const [form, setForm] = useState({
    name: '',
    email: '',
    password: '',
    role: 'Employee', // Varsayılan rol
  });

  const [msg, setMsg] = useState('');             // Kullanıcıya bilgi vermek için mesaj state
  const [loading, setLoading] = useState(false);  // Yükleme durumu

  const handleChange = (e) => {                   // Form alanlarında değişiklik olduğunda çalışır
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {             // Form gönderildiğinde çalışır
    e.preventDefault();                           // Sayfa yenilenmesini engelle
    setMsg('');
    setLoading(true);                             // Buton disable olur

    try {
      // API üzerinden kullanıcı kaydı yapılır
      await api.post('/auth/register', form);

      // Başarılıysa mesaj gösterilir ve 2 saniye sonra giriş sayfasına yönlendirilir
      setMsg("✅ Kayıt başarılı. Giriş sayfasına yönlendiriliyorsunuz...");
      setTimeout(() => navigate('/login'), 2000);
    } catch {
      setMsg("❌ Kayıt başarısız."); // Hata mesajı
    } finally {
      setLoading(false); // Buton tekrar aktif edilir
    }
  };

  return (
    <>
      <Navbar />
      <div style={{ padding: '20px', maxWidth: '500px', margin: '0 auto' }}>
        <h2>📝 Kayıt Ol</h2>

        {/* Hata veya bilgi mesajı varsa göster */}
        {msg && <p style={{ color: msg.startsWith("✅") ? 'green' : 'red' }}>{msg}</p>}
        
        {/* Kayıt formu */}
        <form onSubmit={handleSubmit}>
          {/* Ad Soyad alanı */}
          <input
            name="name"
            type="text"
            placeholder="Ad Soyad"
            value={form.name}
            onChange={handleChange}
            required
            style={{ width: '100%', padding: '10px', marginBottom: '10px' }}
          />

          {/* E-posta alanı */}
          <input
            name="email"
            type="email"
            placeholder="E-posta"
            value={form.email}
            onChange={handleChange}
            required
            style={{ width: '100%', padding: '10px', marginBottom: '10px' }}
          />

          {/* Şifre alanı */}
          <input
            name="password"
            type="password"
            placeholder="Şifre"
            value={form.password}
            onChange={handleChange}
            required
            style={{ width: '100%', padding: '10px', marginBottom: '10px' }}
          />

          {/* Rol seçimi (Admin, Manager, Employee) */}
          <select
            name="role"
            value={form.role}
            onChange={handleChange}
            style={{ width: '100%', padding: '10px', marginBottom: '15px' }}
          >
            <option value="Employee">👨‍💼 Employee</option>
            <option value="Manager">👔 Manager</option>
            <option value="Admin">🛡️ Admin</option>
          </select>

          {/* Kaydol butonu */}
          <button
            type="submit"
            disabled={loading}
            style={{
              width: '100%',
              padding: '12px',
              backgroundColor: '#007bff',
              color: 'white',
              border: 'none',
              cursor: loading ? 'not-allowed' : 'pointer'
            }}
          >
            {loading ? 'Kaydediliyor...' : 'Kaydol'}
          </button>
        </form>
      </div>
    </>
  );
}

export default RegisterPage;

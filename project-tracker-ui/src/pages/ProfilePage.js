import Navbar from '../components/Navbar';
import { useState, useEffect } from 'react';
import api from '../api/axios';
import { useNavigate } from 'react-router-dom';
import { getUserFromToken } from '../utils/auth';

function ProfilePage() {
  const user = getUserFromToken();          
  const navigate = useNavigate();

  // Şifre değişimi için form alanları
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  // Kullanıcıya bilgi vermek için mesaj ve loading durumu
  const [msg, setMsg] = useState('');     
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!user) {
      navigate('/login');
    }
  }, [user, navigate]);

  // Şifre değiştirme isteği
  const handlePasswordChange = async (e) => {
    e.preventDefault();
    setMsg('');

    // Alanlar boş mu kontrolü
    if (!newPassword || !confirmPassword) {
      return setMsg('❗ Lütfen tüm alanları doldurun.');
    }
    // Şifreler eşleşiyor mu kontrolü
    if (newPassword !== confirmPassword) {
      return setMsg('❗ Şifreler eşleşmiyor.');
    }

    try {
      setLoading(true);

      // Şifre güncelleme API isteği (PUT)
      await api.put(
        `/auth/change-password/${user.id}`,
        JSON.stringify(newPassword),         
        { headers: { 'Content-Type': 'application/json' } }
      );

      setMsg('✅ Şifre başarıyla güncellendi.');
      setNewPassword('');
      setConfirmPassword('');
    } catch {
      setMsg('❌ Şifre değiştirilemedi.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Navbar />
      <div style={{ padding: '20px', maxWidth: '500px', margin: '0 auto' }}>
        <h2>👤 Profil</h2>

        {/* Kullanıcı bilgileri */}
        <p><strong>Ad:</strong> {user?.name}</p>
        <p><strong>Email:</strong> {user?.email}</p>
        <p><strong>Rol:</strong> {user?.role}</p>

        <hr />

        {/* Şifre değiştir */}
        <h3>🔐 Şifre Değiştir</h3>
        {msg && (
          <p style={{ color: msg.startsWith('✅') ? 'green' : 'red' }}>
            {msg}
          </p>
        )}

        <form onSubmit={handlePasswordChange}>
          <input
            type="password"
            placeholder="Yeni şifre"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
          />
          <input
            type="password"
            placeholder="Yeni şifre (tekrar)"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            style={{ width: '100%', padding: '8px', marginBottom: '10px' }}
          />

          <button
            type="submit"
            disabled={loading}
            style={{
              padding: '10px 20px',
              backgroundColor: '#28a745',
              color: 'white',
              border: 'none',
              cursor: loading ? 'not-allowed' : 'pointer',
            }}
          >
            {loading ? 'Güncelleniyor...' : 'Şifreyi Güncelle'}
          </button>
        </form>
      </div>
    </>
  );
}

export default ProfilePage;

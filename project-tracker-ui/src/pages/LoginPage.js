import { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css'; 

function LoginPage() {
  const navigate = useNavigate(); 

  // Kullanıcı giriş formu için state tanımları
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [msg, setMsg] = useState(''); // Hata mesajı göstermek için

  // Giriş formu gönderildiğinde çalışacak fonksiyon
  const handleLogin = async (e) => {
    e.preventDefault(); // Sayfanın yenilenmesini engeller
    try {
      // API'ye e-posta ve şifre ile istek gönderilir
      const res = await axios.post('https://localhost:7040/api/auth/login', {
        email,
        password
      });

      // Gelen JWT token localStorage'a kaydedilir
      localStorage.setItem('token', res.data.token);

      // Giriş başarılıysa projeler sayfasına yönlendirilir
      navigate('/projects');
    } catch (err) {
      // Hata durumunda kullanıcı bilgilendirilir
      console.error("Login hatası:", err);
      setMsg("❌ Giriş başarısız. Bilgileri kontrol edin.");
    }
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h2>🔐 Giriş Yap</h2>

        <form onSubmit={handleLogin}>
          {/* Kayıt yönlendirmesi */}
          <p className="sub-text">
            Hesabınız yok mu? <a href="/register">Kayıt olun</a>
          </p>

          {/* E-posta girişi */}
          <input
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            placeholder="E-posta"
            required
            className="login-input"
          />

          {/* Şifre girişi */}
          <input
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            placeholder="Şifre"
            required
            className="login-input"
          />

          {/* Giriş butonu */}
          <button type="submit" className="login-button">
            Giriş
          </button>

          {/* Hata mesajı gösterimi */}
          {msg && <p className="error-message">{msg}</p>}
        </form>
      </div>
    </div>
  );
}

export default LoginPage;

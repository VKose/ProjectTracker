import { useState } from 'react'; 
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { getUserFromToken } from '../utils/auth'; 
import './Navbar.css'; 

function Navbar() {
  const [menuOpen, setMenuOpen] = useState(false);  //  Mobil menü açık mı?
  const navigate = useNavigate();                   //  Sayfa yönlendirme
  const location = useLocation();                   //  Mevcut URL bilgisini al
  const user = getUserFromToken();                  //  Giriş yapan kullanıcıyı al

  //  Çıkış işlemi
  const handleLogout = () => {
    localStorage.removeItem('token'); // Token'ı temizle
    navigate('/login');               // Giriş sayfasına yönlendir
  };

  //  Mobil menü aç/kapa
  const toggleMenu = () => setMenuOpen(!menuOpen);

  return (
    <nav className="navbar">
      {/* Sayfa başlığı */}
      <div className="nav-logo">📋 Çoklu Müşterili Proje ve Görev Takip Sistemi</div>

      {/* Hamburger menü butonu (mobil görünümde aktif) */}
      <button className="hamburger" onClick={toggleMenu}>☰</button>

      {/* Ortalanmış menü bağlantıları */}
      {user && ( // Sadece kullanıcı varsa menü göster
        <div className={`nav-center ${menuOpen ? 'open' : ''}`}>
          <div className="nav-links">
            {/*  Tüm roller için geçerli menüler */}
            <Link
              to="/projects"
              className={location.pathname === '/projects' ? 'active' : ''}
            >
              Projeler
            </Link>
            <Link
              to="/todos"
              className={location.pathname === '/todos' ? 'active' : ''}
            >
              Görevler
            </Link>

            {/*  Admin ve Manager'a özel menüler */}
            {(user?.role === 'Admin' || user?.role === 'Manager') && (
              <>
                <Link
                  to="/add-project"
                  className={location.pathname === '/add-project' ? 'active' : ''}
                >
                  Proje Ekle
                </Link>
                <Link
                  to="/add-todo"
                  className={location.pathname === '/add-todo' ? 'active' : ''}
                >
                  Görev Ekle
                </Link>
                <Link
                  to="/dashboard"
                  className={location.pathname === '/dashboard' ? 'active' : ''}
                >
                  Dashboard
                </Link>
                <Link
                  to="/customers"
                  className={location.pathname === '/customers' ? 'active' : ''}
                >
                  Müşteriler
                </Link>
              </>
            )}

            {/*  Sadece Admin'e özel kullanıcı yönetimi */}
            {user?.role === 'Admin' && (
              <Link
                to="/users"
                className={location.pathname === '/users' ? 'active' : ''}
              >
                Kullanıcılar
              </Link>
            )}

            {/*  Profil sayfası - tüm roller için açık */}
            <Link
              to="/profile"
              className={location.pathname === '/profile' ? 'active' : ''}
            >
              Profil
            </Link>
          </div>
        </div>
      )}

      {/*  Kullanıcı bilgisi ve çıkış butonu */}
      {user && (
        <div className="nav-right">
          <span>💻 {user.name || user.email}</span>         {/*  Kullanıcı ismi veya e-posta */}
          <button onClick={handleLogout}>Çıkış</button>     {/*  Çıkış yap */}
        </div>
      )}
    </nav>
  );
}

export default Navbar;

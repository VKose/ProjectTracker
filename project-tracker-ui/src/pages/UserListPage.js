import Navbar from '../components/Navbar';
import { useEffect, useState } from 'react';
import api from '../api/axios';
import { useNavigate } from 'react-router-dom';
import { getUserFromToken } from '../utils/auth';

function UserListPage() {
  const [users, setUsers] = useState([]);   //  Kullanıcı listesi
  const [msg, setMsg] = useState('');       //  İşlem mesajı (başarı/hata)
  const [form, setForm] = useState({        //  Form verileri (yeni/güncelleme)
    name: '',
    email: '',
    password: '',
    role: 'Employee',
  });
  const [creating, setCreating] = useState(false);  //  İşlem durumu
  const [editMode, setEditMode] = useState(false);  //  Güncelleme mi yapılıyor?
  const [editId, setEditId] = useState(null);       //  Güncellenecek kullanıcı ID'si
  const navigate = useNavigate();                   
  const user = getUserFromToken();                  

  useEffect(() => {
    if (!user) {                     // Giriş yapılmamışsa login sayfasına gönder
      navigate('/login');
      return;
    }

    if (user?.role !== 'Admin') {   //  Yalnızca Admin erişebilir
      navigate('/dashboard');
      return;
    }

    api.get('/user')                //  Kullanıcıları API'den al
      .then(res => setUsers(res.data))
      .catch(() => setMsg("Kullanıcı listesi alınamadı."));
  }, [navigate, user]);

  const handleDelete = async (id) => {
    if (!window.confirm("Bu kullanıcı silinsin mi?")) return; //  Onay

    try {
      await api.delete(`/user/${id}`);            //  Kullanıcıyı sil
      setUsers(users.filter(u => u.id !== id));   //  Listeden çıkar
    } catch (err) {
      const msg = err.response?.data ?? err.message ?? "Silme işlemi başarısız.";   //  Detaylı hata
      alert(typeof msg === 'string' ? msg : JSON.stringify(msg));
    }
  };

  const handleInputChange = (e) => {
    setForm(prev => ({ ...prev, [e.target.name]: e.target.value }));  //  Form girişlerini güncelle
  };

  const handleUserSubmit = async (e) => {
    e.preventDefault();
    setCreating(true);                     
    setMsg('');

    try {
      if (editMode) {   //  Güncelleme işlemi
        await api.put(`/user/${editId}`, form);
        setMsg("✅ Kullanıcı güncellendi.");
      } else {          //  Yeni kullanıcı ekleme
        await api.post('/auth/register', form);
        setMsg("✅ Yeni kullanıcı eklendi.");
      }

      const res = await api.get('/user'); //  Listeyi yenile
      setUsers(res.data);

      //  Formu sıfırla
      setForm({ name: '', email: '', password: '', role: 'Employee' });
      setEditMode(false);
      setEditId(null);
    } catch {
      setMsg("❌ İşlem başarısız.");
    } finally {
      setCreating(false); //  İşlem tamamlandı
    }
  };

  return (
    <>
      <Navbar /> 
      <div style={{ padding: '20px', maxWidth: '900px', margin: '0 auto' }}>
        <h2>👥 Kullanıcı Yönetimi</h2>
        {msg && <p style={{ color: msg.startsWith('✅') ? 'green' : 'red' }}>{msg}</p>} {/*  Bilgilendirme */}

        <form
          onSubmit={handleUserSubmit}
          style={{ marginBottom: '30px', border: '1px solid #ccc', padding: '20px' }}
        >
          <h3>{editMode ? '✏️ Kullanıcıyı Güncelle' : '➕ Yeni Kullanıcı Ekle'}</h3>
          <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            <input
              name="name"
              type="text"
              placeholder="Ad Soyad"
              value={form.name}
              onChange={handleInputChange}
              required
              style={{ flex: 1, padding: '8px' }}
            />
            <input
              name="email"
              type="email"
              placeholder="E-posta"
              value={form.email}
              onChange={handleInputChange}
              required
              style={{ flex: 1, padding: '8px' }}
            />
            <input
              name="password"
              type="password"
              placeholder="Şifre"
              value={form.password}
              onChange={handleInputChange}
              required={!editMode} //  Güncellemede şifre zorunlu değil
              style={{ flex: 1, padding: '8px' }}
            />
            <select
              name="role"
              value={form.role}
              onChange={handleInputChange}
              style={{ flex: 1, padding: '8px' }}
            >
              <option value="Employee">👨‍💼 Employee</option>
              <option value="Manager">👔 Manager</option>
              <option value="Admin">🛡️ Admin</option>
            </select>
            <button
              type="submit"
              disabled={creating}
              style={{
                padding: '10px 20px',
                backgroundColor: editMode ? '#28a745' : '#007bff',
                color: 'white',
                border: 'none'
              }}
            >
              {creating ? (editMode ? 'Güncelleniyor...' : 'Ekleniyor...') : (editMode ? 'Güncelle' : 'Ekle')}
            </button>
          </div>
        </form>

        <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>İsim</th>
              <th>E-posta</th>
              <th>Rol</th>
              <th>İşlem</th>
            </tr>
          </thead>
          <tbody>
            {users.map(u => (
              <tr key={u.id}>
                <td>{u.id}</td>
                <td>{u.name}</td>
                <td>{u.email}</td>
                <td>{u.role}</td>
                <td>
                  <button
                    onClick={() => {
                      //  Kullanıcıyı forma yükle
                      setForm({
                        name: u.name,
                        email: u.email,
                        password: '',
                        role: u.role
                      });
                      setEditMode(true);
                      setEditId(u.id);
                    }}
                    style={{ marginRight: '10px' }}
                  >
                    Düzenle
                  </button>
                  <button
                    onClick={() => handleDelete(u.id)} //  Kullanıcıyı sil
                    style={{ backgroundColor: 'red', color: 'white', padding: '6px 12px', border: 'none' }}
                  >
                    Sil
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}

export default UserListPage;

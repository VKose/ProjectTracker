import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import ProjectsPage from './pages/ProjectsPage';
import TodosPage from './pages/TodosPage';
import PrivateRoute from './components/PrivateRoute'; 
import AddTodoPage from './pages/AddTodoPage';
import AddProjectPage from './pages/AddProjectPage';
import ProfilePage from './pages/ProfilePage';
import DashboardPage from './pages/DashboardPage';
import UserListPage from './pages/UserListPage';
import TodoDetailPage from './pages/TodoDetailPage';
import RegisterPage from './pages/RegisterPage';
import CustomersPage from './pages/CustomersPage';
import { getUserFromToken } from './utils/auth';

function App() {
  const user = getUserFromToken(); // Giriş yapan kullanıcıyı kontrol et

  return (
    // Tarayıcı üzerinde client-side routing işlemlerini başlatır
    // PrivateRoute: Kullanıcının giriş yapıp yapmadığını kontrol eder. JWT token varsa içerik gösterilir, yoksa giriş sayfasına yönlendirir.
    <BrowserRouter>
      <Routes>

        {/* Ana sayfa yönlendirmesi: giriş varsa /projects sayfasına git */}
        <Route path="/" element={
          user ? <Navigate to="/projects" /> : <Navigate to="/login" />
        } />

        {/* Giriş sayfası (herkes erişebilir) */}
        <Route path="/login" element={<LoginPage />} />

        {/* Kayıt sayfası (herkes erişebilir) */}
        <Route path="/register" element={<RegisterPage />} />

        {/* Projeler sayfası (sadece giriş yapmış kullanıcılar) */}
        <Route path="/projects" element={
          <PrivateRoute><ProjectsPage /></PrivateRoute>
        } />

        {/* Görevler sayfası (sadece giriş yapmış kullanıcılar) */}
        <Route path="/todos" element={
          <PrivateRoute><TodosPage /></PrivateRoute>
        } />

        {/* Yeni görev ekleme sayfası (Admin ve Manager) */}
        <Route path="/add-todo" element={
          <PrivateRoute><AddTodoPage /></PrivateRoute>
        } />

        {/* Yeni proje ekleme sayfası (Admin ve Manager) */}
        <Route path="/add-project" element={
          <PrivateRoute><AddProjectPage /></PrivateRoute>
        } />

        {/* Kullanıcı profil sayfası */}
        <Route path="/profile" element={
          <PrivateRoute><ProfilePage /></PrivateRoute>
        } />

        {/* Yönetim paneli (Admin ve Manager erişebilir) */}
        <Route path="/dashboard" element={
          <PrivateRoute><DashboardPage /></PrivateRoute>
        } />

        {/* Kullanıcı listesi (Admin) */}
        <Route path="/users" element={
          <PrivateRoute><UserListPage /></PrivateRoute>
        } />

        {/* Belirli bir görevin detay sayfası */}
        <Route path="/todos/:id" element={
          <PrivateRoute><TodoDetailPage /></PrivateRoute>
        } />

        {/* Müşteriler sayfası (Admin ve Manager) */}
        <Route path="/customers" element={
          <PrivateRoute><CustomersPage /></PrivateRoute>
        } />

      </Routes>
    </BrowserRouter>
  );
}

export default App;

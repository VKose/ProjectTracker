import { Navigate } from 'react-router-dom';

function PrivateRoute({ children }) {
  const token = localStorage.getItem('token');

  // Token yoksa giriş sayfasına yönlendir
  if (!token) {
    return <Navigate to="/login" />;
  }

  // Token varsa bileşeni göster (ProjectsPage, TodosPage vb.)
  return children;
}

export default PrivateRoute;

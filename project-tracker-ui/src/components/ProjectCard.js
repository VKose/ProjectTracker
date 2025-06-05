import './ProjectCard.css';
import { getUserFromToken } from '../utils/auth'; 

function ProjectCard({ project, users, customers, onAssignUser, onAssignCustomer }) {
  const user = getUserFromToken(); // Giriş yapan kullanıcıyı al

  // Manager kendi projesinin yöneticisi mi kontrolü
  const isManagerOfProject =
    user?.role === 'Manager' &&
    Array.isArray(project.users) &&
    project.users.some(u => parseInt(u.id) === parseInt(user.id));

  // Kullanıcı atama yetkisi: Admin her zaman, Manager ise sadece kendi projesine
  const canAssignUsers = user?.role === 'Admin' || isManagerOfProject;

  // Müşteri atama yetkisi (Sadece Admin)
  const canAssignCustomer = user?.role === 'Admin';

  return (
    <div className="project-card">
      <h3>📁 {project.title}</h3>
      <p><strong>Açıklama:</strong> {project.description || '—'}</p>
      <p><strong>Durum:</strong> {project.status}</p>
      <p><strong>Müşteri:</strong> {project.customer?.name || '—'}</p>
      <p><strong>Ekip:</strong> {Array.isArray(project.users) && project.users.length > 0
        ? project.users.map(u => u.name).join(', ')
        : '—'}
      </p>

      {/* Kullanıcı ve müşteri atama alanları */}
      {(canAssignUsers || canAssignCustomer) && (
        <div className="card-actions">
          {/* Kullanıcı atama dropdown - sadece yetkili ise görünür */}
          {canAssignUsers && (
            <select onChange={e => onAssignUser(project.id, e.target.value)}>
              <option value="">👤 Kullanıcı Ata</option>
              {users.map(u => (
                <option key={u.id} value={u.id}>{u.name} ({u.role})</option>
              ))}
            </select>
          )}

          {/* Müşteri atama dropdown - sadece Admin görebilir */}
          {canAssignCustomer && (
            <select onChange={e => onAssignCustomer(project.id, e.target.value)}>
              <option value="">🏢 Müşteri Ata</option>
              {customers.map(c => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          )}
        </div>
      )}
    </div>
  );
}

export default ProjectCard;

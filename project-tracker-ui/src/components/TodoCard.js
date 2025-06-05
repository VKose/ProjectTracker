import { Link } from 'react-router-dom';
import './TodoCard.css';

function TodoCard({ todo, users, onStatusChange, onUserAssign, canAssignUser }) {
  return (
    <div className="todo-card">
      {/* Görev başlığı link olarak gösterilir */}
      <Link to={`/todos/${todo.id}`} className="todo-title-link">
        <h3>📌 {todo.title}</h3>
      </Link>

      <p><strong>Açıklama:</strong> {todo.description || '—'}</p>
      <p><strong>Durum:</strong> {todo.status}</p>
      <p><strong>Teslim Tarihi:</strong> {new Date(todo.dueDate).toLocaleDateString()}</p>
      <p><strong>Proje:</strong> {todo.project?.title || '—'}</p>
      <p><strong>Sorumlu:</strong> {todo.assignedUser?.name || '—'}</p>

      <div className="card-actions">
        {/* Her kullanıcı durum güncelleyebilir */}
        <select value={todo.status} onChange={e => onStatusChange(todo.id, e.target.value)}>
          {["Bekliyor", "Devam Ediyor", "Tamamlandı"].map(status => (
            <option key={status} value={status}>{status}</option>
          ))}
        </select>

        {/* Kullanıcı atama sadece Admin veya Manager */}
        {canAssignUser && (
          <select
            value={todo.assignedUserId || ''}
            onChange={e => onUserAssign(todo.id, e.target.value)}
          >
            <option value="">👤 Kullanıcı Ata</option>
            {users.map(u => (
              <option key={u.id} value={u.id}>{u.name} ({u.role})</option>
            ))}
          </select>
        )}
      </div>
    </div>
  );
}

export default TodoCard;

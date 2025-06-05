import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';
import Navbar from '../components/Navbar';
import { getUserFromToken } from '../utils/auth';
import TodoCard from '../components/TodoCard';

function TodosPage() {
  const [todos, setTodos] = useState([]);
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const user = getUserFromToken();

  // Kullanıcı yoksa girişe yönlendir
  useEffect(() => {
    if (!user) {
      navigate('/login');
    }
  }, [user, navigate]);

  // Görevleri getir
  const fetchTodos = useCallback(async () => {
    try {
      const res = await api.get('/todo');
      setTodos(res.data);
    } catch (err) {
      if (err.response?.status === 401) navigate('/login');
      else setError('Görevler alınamadı.');
    }
  }, [navigate]);

  // Atanabilir kullanıcıları getir
  const fetchUsers = useCallback(async () => {
    try {
      const res = await api.get('/user/assignable');
      setUsers(res.data);
    } catch {
      // Hata alınsa bile sessiz geç
    }
  }, []);

  useEffect(() => {
    fetchTodos();

    if (user?.role === 'Admin' || user?.role === 'Manager') {
      fetchUsers();
    }
  }, [user, fetchTodos, fetchUsers]);

  // Görev durumu güncelle
  const updateStatus = async (id, newStatus) => {
    try {
      await api.put(`/todo/${id}/status`, JSON.stringify(newStatus), {
        headers: { 'Content-Type': 'application/json' },
      });
      fetchTodos(); // Güncel listeyi çek
    } catch {
      alert("Durum güncellenemedi.");
    }
  };

  // Kullanıcı ata
  const assignUser = async (todoId, userId) => {
    try {
      await api.post(`/todo/${todoId}/assign-user/${userId}`);
      fetchTodos(); // Güncel listeyi çek
    } catch {
      alert("Atama başarısız.");
    }
  };

  // Rol bazlı görev filtreleme
  let filteredTodos = [];

  if (user?.role === 'Admin') {
    filteredTodos = todos;
  } else if (user?.role === 'Manager') {
    filteredTodos = todos.filter(todo =>
      (todo.project?.users?.some(u => u.id === user.id)) ||
      todo.assignedUserId === user.id
    );
  } else {
    filteredTodos = todos.filter(todo => todo.assignedUserId === parseInt(user?.id));
  }

  const canAssignUser = user?.role === 'Admin' || user?.role === 'Manager';

  return (
    <>
      <Navbar />
      <div style={{ padding: '20px' }}>
        <h2>Görevlerim</h2>
        {error && <p style={{ color: 'red' }}>{error}</p>}

        <div>
          {filteredTodos.map(todo => (
            <TodoCard
              key={todo.id}
              todo={todo}
              users={users}
              onStatusChange={updateStatus}
              onUserAssign={assignUser}
              canAssignUser={canAssignUser}
            />
          ))}
        </div>
      </div>
    </>
  );
}

export default TodosPage;

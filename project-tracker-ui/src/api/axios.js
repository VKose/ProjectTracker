import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7040/api', // API sunucunun adresi
});

// Her istekten önce token ekle (eğer varsa)
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token'); // JWT token tarayıcıda saklanmış mı?
  if (token) {
    config.headers = config.headers || {}; // headers nesnesi yoksa oluştur
    config.headers.Authorization = `Bearer ${token}`; // Header'a ekle
  }
  return config;
});

export default api;

import { jwtDecode } from 'jwt-decode';

// LocalStorage'daki JWT token'dan kullanıcı bilgilerini çözüp döndürür
export function getUserFromToken() {
  const token = localStorage.getItem('token'); // Token'ı al
  if (!token) return null; // Eğer yoksa null döndür (giriş yapılmamış)

  try {
    const decoded = jwtDecode(token); // Token'ı çözümle

    // Token'dan alınan claim'ler üzerinden kullanıcı bilgilerini dön
    return {
      id: parseInt(
        decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
      ), // Kullanıcı ID
      email: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
      role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"], 
      name: decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] 
    };
  } catch (err) {
    console.warn("Geçersiz JWT token:", err);
    return null; // Hatalı token varsa null dön
  }
}

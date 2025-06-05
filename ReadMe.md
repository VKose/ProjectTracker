***Çoklu Müşterili Proje ve Görev Takip Sistemi***

Bu uygulama, çok müşterili bir yapıya sahip proje ve görev takip sistemidir. Admin, Manager ve Employee rolleri ile yetkilendirme yapılır. React.js frontend ve .NET Web API backend mimarisiyle geliştirilmiştir.
Şirketlerin birden fazla müşteriye hizmet verdiği projeleri, bu projelere atanmış ekipleri, görevleri ve süreçleri merkezi bir sistemden yönetmesini sağlamak amaçlanmıştır.
Uygulama, proje yöneticileri, çalışanlar ve yönetim arasında görevlerin takibini, termin kontrolünü ve durum analizlerini kolaylaştırmayı amaçlar.
Farklı rollerle kullanıcıların erişim ve yetki düzeyleri kısıtlanarak kontrollü bir yönetim altyapısı sunar.

-----

**Proje Yapısı

ProjectTracker/
── ProjectTrackerAPI/ # .NET Backend
──| Controllers/
──|Models/
──| Data/AppDbContext.cs
──|Services/EmailService.cs #SMTP Mail Sistemi

── project-tracker-ui/ # React Frontend
── | components/
── |pages/
── |api/axios.js
── |utils/auth.js
── |App.js

**Veritabanı Yapısı (Microsoft SQL Server)

Users – Kayıtlı sistem kullanıcıları
Projects – Her biri bir müşteriye ait projeler
Todos – Görevler (bir projeye ve kullanıcıya atanabilir)
Customers – Proje sahibi müşteri firmalar

User - Projects → many-to-many (bir kullanıcı birden fazla projede olabilir)
Projects - Todo → one-to-many
Customer - Projects → one-to-many

** Özellikler

-  JWT tabanlı kimlik doğrulama
-  Rol bazlı yetkilendirme (Admin, Manager, Employee)
-  Görev atama ve durum güncelleme
-  Proje oluşturma ve kullanıcı atama
-  Müşteri yönetimi (CRUD)
-  Yönetim Paneli (Dashboard)
-  E-posta bildirim gönderimi 
-  Mobil uyumlu kullanıcı arayüzü
-  Parolalar SHA256 ile hash’lenerek saklanır.
-  Silme işlemlerinde referans kısıtlamalarına dikkat edilir.
-  Kullanıcı silinemiyorsa, atanmış görevler kontrol edilmelidir.
-  React’te PrivateRoute ile korumalı sayfa erişimi sağlanır.
-  Proje içindeki simgeler doğrudan "emoji" olarak HTML içinde kullanılmıştır.

-----

** Kullanılan Teknolojiler

* Backend :

- .NET Web API
- Entity Framework Core
- Microsoft SQL Server
- JWT (JSON Wen Token)  --> JSON Web Token (JWT), tarafların birbirleri arasındaki veri alışverişini ve bunun doğrulamasını sağlamaktadır.
- Swagger  --> Rest API'lerin incelenmesi, anlaşılması ve test edilmesini sağlayan bir arayüz sağlamaktadır.

* Frontend :

- React.js
- React Router  --> React uygulamalarında sayfa yönetimini kolaylaştırmak ve kullanıcıların farklı sayfalar arasında gezinmelerini sağlamak için kullanılan bir JavaScript kütüphanesidir.
- Axios  --> Bir sunucuya veri almak veya göndermek için HTTP istekleri yapmak için kullanılan JavaScript için söz tabanlı bir HTTP istemcisidir.
- CSS

-----

*****KURULUM

* CMD üzerinde kullanılabilecek komutlar (NuGet Paketleri, Veritabanı İşlemleri vs.)

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet tool install --global dotnet-ef   --> dotnet ef komutu yoksa
dotnet ef migrations add InitialCreate
dotnet ef database update

cd ProjectTrackerAPI 
dotnet restore
dotnet ef database update
dotnet run

cd client
npm install
npm start


*****KULLANIM

***Backend

1) Terminalde ProjectTrackerAPI klasörüne girin.
cd ProjectTrackerAPI

2) Gerekli NuGet paketlerini yükleyin:
dotnet restore

3) Veritabanını oluşturun:
dotnet ef database update

4) API'yi başlatın:
dotnet run

5) Tarayıcıdan Swagger arayüzüne gidin:
https://localhost:7040/swagger (port numarası farklılık gösterebilir)

6) JWT Token almak için:

POST /api/auth/login → token alın.

Swagger'da "Authorize" butonuna tıklayıp Bearer {token} girin.
İlgili label'a yalnızca login olduğunuzda tanımlanan token girilir. NOT: Şu an 1 saat geçerli token üretiliyor, değiştirilebilir.

***Frontend

1) Terminalde client klasörüne girin:
cd project-tracker-ui (client)

2) Gerekli paketleri kurun:
npm install

3) React uygulamasını başlatın:
npm start

4) Tarayıcıda açın:
http://localhost:3000 (port numarası farklılık gösterebilir)


***Görev Bildirim Mail Servisi

DailyTaskNotifier.cs dosyası bir arka plan servistir.
Her gün saat 09:00'da, ertesi gün teslim tarihi olan görevler için kullanıcıya otomatik e-posta gönderimi yapar.

E-postaların gönderilebilmesi için:
EmailService.cs yapılandırılmalı
appsettings.json içinde SMTP ayarları (Host, Port, Username, Password) doğru girilmelidir.
NOT : E-posta için mailtrap kullanıldı.

-----

***Sistem Erişimi için örnek admin girişi ;

admin@example.com	
123456 (tüm kullanıcıların şifreleri aynı)

***API Üzerinden HTTP Metodu Giriş Örnekleri ;

POST /api/customer → Yeni Müşteri Ekleme
{
  "name": "Abcd A.Ş.",
  "contactInfo": "info@abcd.com"
}

PUT /api/customer/1 → Müşteri Güncelleme
{
  "id": 1,
  "name": "Abcd Güncellenmiş",
  "contactInfo": "guncel@abcd.com"
}

gibi...

-----


**Kullanıcı Rolleri
	    
Admin	    : Tüm sistem yönetimi ve kullanıcı erişimleri
Manager	  : Sorumlu olduğu projeleri ve ekip üyelerini yönetir.
Employee	: Sadece kendisine atanan görevleri görüntüler ve günceller.


**Sayfa Düzeni

/login	         : Kullanıcı girişi
/register	       : Yeni kullanıcı kaydı (sadece Admin için)
/projects	       : Proje listesi ve kullanıcı/müşteri atamaları
/todos	         : Görev listesi, atama ve durum güncelleme
/add-todo	       : Yeni görev oluşturma (Admin/Manager)
/add-project     : Yeni proje oluşturma (Admin/Manager)
/dashboard       : Özet gösterge paneli (Admin/Manager(Kısıtlı))
/users	         : Kullanıcı yönetimi (Admin)
/customers       : Müşteri yönetimi (Admin/Manager)
/profile         : Kullanıcının kendi bilgileri


-----


***.NET Web API 

AuthController	     : Giriş & Kayıt işlemleri
UserController	     : Kullanıcı CRUD
ProjectController	   : Proje CRUD & kullanıcı atama
TodoController	     : Görev CRUD, atama, durum takibi
CustomerController	 : Müşteri CRUD işlemleri

HTTP Metotları

GET     : Serverda bulunan kaynağa erişmek amacıyla kullanılmaktadır.
POST    : Server tarafında yeni bir kaynağn oluşturulması için kulanılmaktadır.
PUT     : Server tarafında yeni bir kaynağın oluşturulması veya varolan kaynağın tümüyle update edilmesi için kullanılır.
DELETE  : Server tarafındaki kaynağın silinmesi için kullanılmaktadır.

***React.js

Navbar 		      : Rol bazlı görünür bağlantılar
ProjectsPage 	  : Proje listesi, kullanıcı & müşteri atama
TodosPage	      : Görev listesi, durum güncelleme & atama
DashboardPage 	: İstatistikler & analiz paneli (rol bazlı)
UserListPage	  : Admin’e özel kullanıcı yönetimi
CustomersPage 	: Müşteri listesi ve işlemleri

-----

****API Dökmantasyonu

Kullanıcı Girişi & Kayıt

POST/api/auth/register				      Yeni kullanıcı kaydı
POST/api/auth/login				          Giriş yap ve JWT token al
PUT	/api/auth/change-password/{id}	Parola değiştirme

Müşteri (Customer) İşlemleri

GET/api/customer				    Tüm müşterileri getir
GET/api/customer/{id}				Belirli müşteriyi getir
POST/api/customer				    Yeni müşteri oluştur (Admin/Manager)
PUT/api/customer/{id}				Müşteri bilgilerini güncelle
DELETE/api/customer/{id}		Müşteriyi sil (Sadece Admin)

Proje İşlemleri

GET/api/project				                              Tüm projeleri getir
GET/api/project/{id}				                        Proje detaylarını getir
POST/api/project				                            Yeni proje oluştur (Admin/Manager)
PUT/api/project/{id}				                        Proje güncelle (Admin/Manager)
DELETE/api/project/{id}				                      Proje sil (Sadece Admin)
POST/api/project/{id}/assign-user/{userId}		      Projeye kullanıcı ata (Admin/Manager)
POST/api/project/{id}/assign-customer/{customerId}	Projeye müşteri ata (Sadece Admin)

Görev (Todo) İşlemleri

GET/api/todo					                    Görevleri getir
POST/api/todo					                    Yeni görev ekle
PUT/api/todo/{id}				                  Görevi güncelle
DELETE/api/todo/{id}				              Görevi sil
PUT/api/todo/{id}/status				          Görev durumunu güncelle
POST/api/todo/{id}/assign-user/{userId}		Göreve kullanıcı ata
POST/api/todo/{id}/notify				          Bilgilendirme maili

Kullanıcı İşlemleri (Sadece Admin)

GET/api/user					      Tüm kullanıcıları getir
DELETE/api/user/{id}				Belirli kullanıcıyı sil
PUT/api/user/{id}				    Kullanıcı bilgilerini güncelle
GET/api/user/assignable			Atanabilir kullanıcıları getir (Admin: tümü, Manager: sadece Employee)


-----


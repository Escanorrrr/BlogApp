# Blog Uygulaması

Bu proje, .NET Core teknolojileri kullanılarak geliştirilmiş kapsamlı bir blog platformudur. MVC mimarisi ve Entity Framework Core kullanılarak oluşturulmuş, kullanıcı yönetimi ve blog yazıları yönetimi gibi temel özellikleri içermektedir.

## Özellikler

### 1. Kullanıcı Yönetimi
- ✅ Kullanıcı kaydı ve girişi
- ✅ JWT tabanlı kimlik doğrulama
- ✅ Rol bazlı yetkilendirme (Admin/User)
- ✅ Güvenli şifre yönetimi

### 2. Blog Yazıları
- ✅ Blog yazısı oluşturma, okuma, güncelleme ve silme (CRUD)
- ✅ Kategori sistemi
- ✅ Resim yükleme desteği
- ✅ Yazar bilgisi ve yayın tarihi

### 3. Kategoriler
- ✅ Kategori bazlı filtreleme


### 4. Arayüz
- ✅ Modern ve responsive tasarım
- ✅ Bootstrap 5 entegrasyonu
- ✅ Kullanıcı dostu navigasyon
- ✅ Dinamik içerik yükleme

## Teknolojiler

- 🔷 ASP.NET Core 8.0
- 🔷 Entity Framework Core 8.0
- 🔷 MS SQL Server
- 🔷 JWT (JSON Web Token)
- 🔷 Bootstrap 5
- 🔷 Razor Pages
- 🔷 Repository Pattern
- 🔷 SOLID Prensipleri


## Veritabanı Yapısı

### ER Diyagramı
![Veritabanı ER Diyagramı](images/db-diagrams.png)

### Tablolar

#### Users
- Id (int)
- Username (string)
- Email (string)
- PasswordHash (string)
- IsAdmin (bool)
- CreatedAt (DateTime)
- ImagePath (string)
- IsBlocked (bool)

#### BlogPosts
- Id (int)
- Title (string)
- Content (string)
- ImagePath (string)
- PublishedDate (DateTime)
- UpdatedDate (DateTime)
- UserId (int, FK)
- CategoryId (int, FK)

#### Categories
- Id (int)
- Name (string)

#### Comments
- Id (int)
- Content (string)
- CreatedAt (DateTime)
- UserId (int, FK)
- BlogPostId (int, FK)

- ## Ekran Görüntüleri

### Ana Sayfa
![Ana Sayfa](images/ekran.png)

### Blog Detay Sayfası
![Blog Detay](images/edit.png)

## Çoklu Proje Başlatma Ayarları

1. Solution Explorer'da solution'a (BlogApp) sağ tıklayın
2. "Set Startup Projects..." seçeneğini seçin
3. "Multiple startup projects" seçeneğini işaretleyin
4. Listeden:
   - BlogApp (API) -> Action: "Start"
   - BlogApp.Web -> Action: "Start"
   olarak ayarlayın
5. Apply ve OK diyerek kaydedin

Artık Visual Studio'da projeyi çalıştırdığınızda (F5 veya Ctrl+F5) hem API hem de Web projesi aynı anda başlayacaktır.

## Güvenlik

- JWT token bazlı kimlik doğrulama
- Şifrelerin güvenli bir şekilde hashlenmesi
- CORS politikası yapılandırması
- XSS ve CSRF koruması

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakınız.


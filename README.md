# 🌦️ HavamaGore - Mood Based Weather App

Bu proje, **Bursa Uludağ Üniversitesi Yönetim Bilişim Sistemleri Bölümü**, Web Tabanlı Programlama dersi final projesi olarak geliştirilmiştir.

## 🎯 Projenin Amacı
İnsanların ruh hali hava durumundan doğrudan etkilenmektedir. **HavamaGore**, anlık hava durumu verilerini çekerek kullanıcının o anki atmosferine en uygun **Film, Kitap ve Müzik** önerilerini sunan, kişiselleştirilmiş bir web platformudur. Amacı, kullanıcıların "Bugün ne izlesem/okusam?" kararsızlığını hava durumuna dayalı bir algoritma ile çözmektir.

## 👥 Hedef Kullanıcı Kitlesi
* Günlük ne izleyeceğine/okuyacağına karar veremeyenler.
* Hava durumuna göre mod değiştiren ve buna uygun içerik arayanlar.
* Sinema ve edebiyat severler.

## 🛠️ Kullanılan Teknolojiler
* **Dil:** C#
* **Framework:** ASP.NET Core 8.0 MVC
* **Veritabanı:** MS SQL Server (Entity Framework Core - Code First)
* **Front-End:** HTML5, CSS3, Bootstrap 5, JavaScript (Anime.js)
* **API Entegrasyonları:**
    * WeatherAPI (Hava Durumu)
    * TMDB API (Filmler)
    * Google Books API (Kitaplar)
    * Spotify API (Müzikler)

## 💻 Proje Senaryosu ve Ekran Görüntüleri

### 1. Giriş ve Karşılama
Kullanıcı siteye girdiğinde "Bugün Havan Nasıl?" animasyonu ile karşılanır. Tavşanlı konsept tasarım ile modern ve eğlenceli bir giriş ekranı sunulur.

![Giriş Ekranı](https://github.com/user-attachments/assets/9a7efca2-fa85-497f-b352-26aac5b79fc6)

### 2. Ana Sayfa ve Mood Analizi (Keşfet)
Kullanıcı giriş yaptığında, sistem bulunduğu şehrin hava durumunu (Örn: Ankara) otomatik çeker. Havanın durumuna göre (Örn: Gizemli & Sakin) site teması ve önerilen içerik modu anlık olarak değişir.

![Ana Sayfa Hero](https://github.com/user-attachments/assets/f2faddcd-21e3-4fda-a900-44198d11e094)

### 3. Akıllı İçerik Listeleri
Hava durumuna özel olarak filtrelenmiş Film, Müzik ve Kitap önerileri, Netflix benzeri yatay kaydırılabilir modern listeler halinde sunulur.

![Öneri Listeleri](https://github.com/user-attachments/assets/1c6c3368-2e32-4ebd-a6ea-861fdbb95ba8)

### 4. Kütüphanem (CRUD - Create/Read/Delete)
Kullanıcı beğendiği içerikleri "Kalp" ikonuna basarak veritabanına kaydeder. Kütüphanem sayfasında bu içerikler türlerine göre filtrelenebilir ve yönetilebilir.

![Kütüphane Sayfası](https://github.com/user-attachments/assets/4d6fd58e-4e1f-4c3d-b827-e88f56f4c8cc)

### 5. Profil ve İstatistikler (Mood Analizi)
Kullanıcının kaydettiği içeriklere göre hangi mood'da olduğu (Örn: Chill, Pop, Acoustic) grafiksel olarak analiz edilir. Kullanıcı bilgileri buradan güncellenebilir (Update).

![Profil Analizi](https://github.com/user-attachments/assets/5b9cc5af-6a4c-436b-b8fc-f919646b3f5f)

### 6. Sosyal Keşfet (Arkadaş Sistemi)
Diğer kullanıcıların hangi şehirde, hangi hava durumunda olduklarını ve o anki modlarını görebileceğiniz sosyal etkileşim alanı.

![Keşfet Sayfası](https://github.com/user-attachments/assets/721dab34-f1d7-47f1-bd5f-73231e638c19)

---

## 🎥 Tanıtım Videosu
Projenin detaylı anlatımı, kod yapısı ve çalışır halini izlemek için YouTube videoma göz atabilirsiniz:

[![HavamaGore Tanıtım](https://img.youtube.com/vi/bquGKbOlgSg/0.jpg)](https://www.youtube.com/watch?v=bquGKbOlgSg)

[Videoyu İzlemek İçin Tıklayın](https://www.youtube.com/watch?v=bquGKbOlgSg&t=1s)

---
**Geliştirici:** Zehra Okur
**Ders:** Web Tabanlı Programlama

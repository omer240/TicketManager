TICKET MANAGER UYGULAMASI
========================

Bu proje, kullanıcıların ticket (talep) oluşturabildiği, kendisine atanan
veya kendi oluşturduğu ticket’ları yönetebildiği bir Ticket / Request
Management uygulamasıdır.

Proje iki ana bölümden oluşmaktadır:
- Backend: .NET Core Web API
- Frontend: Angular 20

Frontend ve backend katmanları birbirinden bağımsız olarak geliştirilmiş,
RESTful API üzerinden haberleşmektedir.


------------------------------------------------
PROJE YAPISI
------------------------------------------------
/frontend    → Angular 20 tabanlı kullanıcı arayüzü
/backend     → .NET Core Web API
/docs        → Proje notları ve açıklamalar
/postman     → Postman collection ve kullanım rehberi


------------------------------------------------
BACKEND (API) NASIL ÇALIŞTIRILIR?
------------------------------------------------

GEREKSİNİMLER:
- .NET SDK
- Microsoft SQL Server

VERİTABANI AYARLARI:
Backend projesi içerisinde bulunan appsettings.json dosyası GitHub’a eklenmiştir.

Aşağıdaki connection string alanı kendi MSSQL bilgilerinize göre düzenlenebilir:

"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=TicketManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
}

Server adı, kullanıcı adı veya şifre bilgileri buradan değiştirilebilir.


VERİTABANI OLUŞTURMA (MIGRATION):
Backend dizininde aşağıdaki komut çalıştırılarak veritabanı oluşturulabilir:

dotnet ef database update


BACKEND ÇALIŞTIRMA:
Backend uygulaması aşağıdaki komut ile başlatılır:

dotnet run

Varsayılan adres:
https://localhost:7011


------------------------------------------------
FRONTEND (ANGULAR) NASIL ÇALIŞTIRILIR?
------------------------------------------------

GEREKSİNİMLER:
- Node.js (LTS)
- Angular CLI

BAĞIMLILIKLARI YÜKLEME:
Frontend dizinine girerek:

npm install

FRONTEND ÇALIŞTIRMA:
Angular uygulaması aşağıdaki komut ile başlatılır:

ng serve

Uygulama varsayılan olarak aşağıdaki adreste çalışır:
http://localhost:4200


BACKEND API ADRESİ:
Frontend tarafında backend adresi environment dosyası üzerinden tanımlıdır.
Gerekirse aşağıdaki dosyadan backend URL değiştirilebilir:

src/environments/environment.ts


------------------------------------------------
KİMLİK DOĞRULAMA VE YETKİLENDİRME
------------------------------------------------

- Uygulama JWT tabanlı kimlik doğrulama kullanmaktadır.
- Login işlemi sonrası alınan token otomatik olarak saklanır.
- Yetkisiz kullanıcılar korumalı sayfalara erişemez.
- Ticket işlemleri backend tarafındaki iş kurallarına göre yetkilendirilmiştir.


------------------------------------------------
API TESTLERİ (POSTMAN)
------------------------------------------------

- Tüm API endpoint’leri Postman Collection olarak hazırlanmıştır.
- Collection dosyası /postman klasörü altında yer almaktadır.
- Postman üzerinden ticketId ve commentId değerleri otomatik olarak yönetilmektedir.
- API’ler zincirleme ve tekrar çalıştırılabilir şekilde test edilebilir.


------------------------------------------------
GENEL NOTLAR
------------------------------------------------

- Frontend ve backend tamamen ayrık yapıdadır.
- RESTful API prensiplerine uygun olarak geliştirilmiştir.
- Kod yapısı okunabilir, sürdürülebilir ve genişletilebilir şekilde tasarlanmıştır.
- Proje, case gereksinimlerini eksiksiz karşılayacak şekilde tamamlanmıştır.


------------------------------------------------
SONUÇ
------------------------------------------------

Bu proje, backend ve frontend katmanlarıyla birlikte uçtan uca çalışan,
kimlik doğrulama ve yetkilendirme kuralları bulunan profesyonel bir
Ticket Manager uygulamasıdır.

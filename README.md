# CineGo

Tài liệu đặc tả: https://docs.google.com/document/d/1jc8Z7Ue2HkfkHCqV061q0xFbYHCR6uB_RZuhlyW5rPM/edit?usp=sharing
ERD: https://drive.google.com/file/d/1F7SDIYLtt_JK4LNWjFASeMt-nAAPSDZx/view?usp=sharing
https://app.diagrams.net/#G1F7SDIYLtt_JK4LNWjFASeMt-nAAPSDZx#%7B%22pageId%22%3A%22lghIzevpMhp8kppldpKu%22%7D
CineGo/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── MoviesController.cs
│   ├── BookingController.cs
│   ├── PaymentController.cs
│   └── AccountController.cs
│
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       │   ├── MoviesController.cs
│       │   ├── CinemasController.cs
│       │   ├── ShowTimesController.cs
│       │   ├── SeatsController.cs
│       │   ├── UsersController.cs
│       │   └── PaymentsController.cs
│       ├── Views/
│       │   ├── Movies/
│       │   ├── Cinemas/
│       │   ├── ShowTimes/
│       │   ├── Seats/
│       │   ├── Users/
│       │   └── Payments/
│       └── Admin.csproj (nếu tách riêng, không bắt buộc)
│
├── Models/
│   ├── Movie.cs
│   ├── Cinema.cs
│   ├── Region.cs
│   ├── City.cs
│   ├── ShowTime.cs
│   ├── Seat.cs
│   ├── Ticket.cs
│   ├── Payment.cs
│   ├── Review.cs
│   ├── User.cs (nếu không dùng Identity mặc định)
│   └── ApplicationDbContext.cs
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Movies/
│   │   ├── Index.cshtml   (danh sách phim)
│   │   └── Details.cshtml (chi tiết phim)
│   ├── Booking/
│   │   ├── SelectShowTime.cshtml
│   │   ├── SelectSeats.cshtml
│   │   ├── Confirm.cshtml
│   │   └── Success.cshtml
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── Profile.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _LoginPartial.cshtml
│       ├── _ValidationScriptsPartial.cshtml
│       ├── _ViewImports.cshtml
│       └── _ViewStart.cshtml
│
├── Services/
│   ├── Interfaces/
│   │   ├── IMovieService.cs
│   │   ├── IBookingService.cs
│   │   └── IPaymentService.cs
│   ├── Implementations/
│   │   ├── MovieService.cs
│   │   ├── BookingService.cs
│   │   └── PaymentService.cs
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── lib/ (Bootstrap, jQuery…)
│
├── appsettings.json
├── Program.cs
├── Startup.cs (nếu .NET 5 trở xuống, còn .NET 6+ thì tích hợp vào Program.cs)
└── CineGo.csproj

===============================================================================================================

Controllers/: chứa controller cho khách hàng (frontend).
Areas/Admin/: khu vực riêng dành cho Admin (có controller & view riêng).
Models/: chứa entity (EF Core) + DTO (nếu cần).
Views/: giao diện của khách hàng, chia theo controller.
Shared/: chứa layout, partial view dùng chung.
Services/: business logic (đặt vé, xử lý thanh toán, gửi email/SMS OTP).
wwwroot/: static files (CSS, JS, images).
appsettings.json: connection string + config (payment API, email server…).

===============================================================================================================
🧭 Thứ tự công việc chi tiết cho dự án CineGo
🩵 Giai đoạn 1: Chuẩn bị & Khởi tạo

Tạo project ASP.NET Core Web App (Model-View-Controller)

Tên: CineGo

Chọn .NET 8 hoặc .NET 9 (nếu đã có).

Cấu trúc lại thư mục dự án theo chuẩn (Controllers, Models, Views, Areas/Admin, Services, wwwroot...).

Cài đặt các package cần thiết

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.Extensions.Configuration.UserSecrets


Cấu hình Connection String trong appsettings.json.

🩵 Giai đoạn 2: Thiết kế CSDL & Entity

Vẽ ERD (sơ đồ cơ sở dữ liệu):

Bảng chính: Movie, Cinema, City, Region, ShowTime, Seat, Ticket, User, Payment, Review.

Tạo Models tương ứng trong thư mục Models/

Khai báo các thuộc tính (Id, Name, Date, Type, Price, …).

Xác định quan hệ (1-n, n-n).

Tạo ApplicationDbContext.cs

Kế thừa DbContext hoặc IdentityDbContext<User>.

Đăng ký DbSet cho các bảng.

Thực hiện Migration

dotnet ef migrations add InitialCreate
dotnet ef database update

🩵 Giai đoạn 3: Xây dựng phần Admin (quản trị hệ thống)

Tạo khu vực quản trị (Areas/Admin)

Controller: MoviesController, CinemasController, ShowTimesController, SeatsController, UsersController.

View: CRUD cho từng entity (Index, Create, Edit, Delete).

Dùng scaffold để sinh nhanh CRUD:

dotnet aspnet-codegenerator controller -name MoviesController -m Movie -dc ApplicationDbContext --relativeFolderPath Areas/Admin/Controllers --useDefaultLayout --referenceScriptLibraries


Tích hợp Identity cho Admin

Tạo tài khoản admin mặc định khi seed dữ liệu.

Gán role Admin.

Xây dựng layout riêng cho Admin (Areas/Admin/Views/Shared/_Layout.cshtml).

Sidebar quản lý: Phim, Rạp, Suất chiếu, Ghế, Người dùng, Giao dịch.

🩵 Giai đoạn 4: Xây dựng phần Khách hàng (frontend)

HomeController + Views/Home

Trang chủ, banner phim hot.

MoviesController

Danh sách phim đang chiếu / sắp chiếu.

Trang chi tiết phim (có trailer, review).

BookingController

Chọn khu vực → rạp → ngày → phim → suất → ghế.

Chọn số lượng vé → xác nhận → thanh toán.

AccountController

Đăng ký, đăng nhập (qua email/sđt).

Xác minh OTP/email.

Hồ sơ khách hàng, lịch sử mua vé, huỷ vé.

PaymentController

Giả lập thanh toán (có thể dùng Stripe sandbox / MoMo sandbox).

Hiển thị vé (barcode/QR code).

🩵 Giai đoạn 5: Bổ sung Tính năng Phụ

Review & Rating phim.

Bộ lọc phim (2D/3D, độ tuổi, thể loại).

Gửi email xác nhận đặt vé.

Hoàn tiền trong 48h (cập nhật trạng thái vé).

Quản lý thống kê cho Admin (doanh thu, vé bán ra).

🩵 Giai đoạn 6: UI/UX & Testing

Dùng Bootstrap hoặc TailwindCSS.

Thêm icon, hiệu ứng, validation client-side.

Unit test (service logic).

Integration test (controller).

Manual test flow đặt vé hoàn chỉnh.

🩵 Giai đoạn 7: Triển khai (Deploy)

Deploy lên IIS / Azure / Docker.

Cấu hình:

Logging, error handling.

Connection string production.

Backup database.

✅ Tóm tắt lộ trình
Giai đoạn	Nội dung	Mục tiêu
1	Tạo project + setup EF Core	Chuẩn bị nền tảng
2	Thiết kế database + entity	Có dữ liệu để làm việc
3	Xây dựng Admin	Nhập và quản lý dữ liệu
4	Xây dựng trang khách hàng	Cho phép người dùng đặt vé
5	Tính năng mở rộng	Tăng trải nghiệm
6	UI/UX + Testing	Hoàn thiện sản phẩm
7	Deploy	Đưa hệ thống vào hoạt động
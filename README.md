# MISA.Fresher (Backend)

Tóm tắt ngắn

- Backend: .NET8 (C#)
- Kiến trúc:3 tầng (API / Infrastructure / Core)
- DB mẫu: MySQL (schema: `customer.sql`)

---

## Yêu cầu (Prerequisites)

- .NET8 SDK
- Visual Studio2022 hoặc VS Code + C# extension
- MySQL server
- (Tuỳ chọn) Git, Postman

---

## Build & chạy (local)

1. Restore & build:
 - `dotnet restore`
 - `dotnet build`

2. Chạy API:
 - `dotnet run --project MISA.Fresher.API`
 - Hoặc mở solution trong Visual Studio và chạy (F5)

3. Seeder (tạo CSV mẫu):
 - File mẫu: `customers_seed.csv` trong ContentRoot project API. (1000 bản ghi,5% sdt trùng,30% mua hàng trong30 ngày gần nhất)

---

## Cấu trúc dự án (tóm tắt)

- `MISA.Fresher.API` — Controllers, middleware, Program.cs
- `MISA.Fresher.Core` — Entities, DTOs, Interfaces, Services (business logic), configuration.
- `MISA.Fresher.Infrastructure` — Repositories (Dapper), Seeders, File storage implementation.

---

## Kiến trúc3 tầng (Chi tiết, rõ ràng)

Hệ thống được tổ chức theo mô hình3 tầng (3-layer) rõ ràng — mỗi tầng có trách nhiệm riêng:

1) Presentation Layer (API)
 - Thành phần: `MISA.Fresher.API` (Controllers, Middlewares)
 - Trách nhiệm:
 - Tiếp nhận yêu cầu HTTP, parse input (query, body, files)
 - Gọi các service trong tầng Business để thực thi nghiệp vụ
 - Chuẩn hóa response (ApiResponse) và quản lý lỗi qua `ExceptionMiddleware`
 - Cung cấp endpoint import/export, upload avatar (sử dụng `IFileStorageService`)
 - Lợi ích:
 - Tách biệt giao tiếp HTTP khỏi logic nghiệp vụ

2) Business Layer (Core)
 - Thành phần: `MISA.Fresher.Core.Services` (ví dụ `CustomerService`), `BaseService<T>`
 - Trách nhiệm:
 - Định nghĩa luật nghiệp vụ: validate dữ liệu, transform DTO → Entity
 - Quản lý tương tác nghiệp vụ phức tạp (partial success khi import)
 - Gọi Repository để lưu/đọc dữ liệu
 - Trừu tượng hoá các tác vụ dùng chung (tạo mã, validate custom attributes)
 - Lợi ích:
 - Tập trung mọi logic nghiệp vụ, dễ test unit, tái sử dụng

3) Data Access Layer (Infrastructure)
 - Thành phần: `MISA.Fresher.Infrastructure.Repositories` (Dapper-based repositories)
 - Trách nhiệm:
 - Thao tác trực tiếp với database (SQL, stored procedures)
 - Thực hiện các truy vấn, insert, update, delete
 - Cung cấp interface (ICustomerRepository, IBaseRepository) để Business layer gọi
 - Lợi ích:
 - Tách biệt chi tiết truy cập dữ liệu, dễ thay đổi DB engine hoặc tối ưu truy vấn

Luồng điển hình (ví dụ import CSV):
- Client upload CSV → Controller nhận file → gọi `CustomerService.ImportCustomersFromCSV`.
- `CustomerService` đọc file theo streaming (CsvHelper): `while(csv.Read())` → validate mỗi dòng → gọi `_customerRepository.Insert` (hoặc batch) để lưu.
- Nếu có lỗi từng dòng, service thu lại `ImportError` để trả về partial result.

---

## Database

- Schema mẫu: `customer.sql` (bảng `misa_crm_2025.customer`).
---

## File storage

- Interface: `IFileStorageService` (Core)
- Implementation: `LocalFileStorageService` (Infrastructure) — lưu vào folder cấu hình bởi `FileStorageOptions.RootPath` (mặc định `wwwroot`).

---

## Import / Export CSV

- Export: `CustomerService.ExportCustomersToCSV` trả file bytes.
- Import: streaming xử lý, tạo `CustomerImportDto` per-row cho mục đích báo lỗi (không giữ list toàn bộ trong bộ nhớ).
- Lưu ý với Excel: mã số, số điện thoại được format/đưa ra sao để giữ số0 đầu (project đang thêm dấu `,` hoặc `="..."` theo cấu hình) — cân nhắc chuẩn hoá khi import.

---

## Postman & testing nhanh

Tạo environment `Development` với biến:
- `baseUrl` = `https://localhost:5001` (hoặc `http://localhost:5000` / `http://localhost:5198` tùy port)

Collection gợi ý: tạo folder `Customers` và các request dưới đây.

###1) GET — Lấy trang khách hàng (Paging + filter + sort)
- Name: `Customers - Get Paging`
- Method: GET
- URL: `{{baseUrl}}/api/customer/paging`
- Example full URL (ví dụ bạn đưa):
 `http://localhost:5198/api/customer/paging?search=0912113555&page=1&pageSize=10&sortBy=&sortDirection=`
- Query params:
 - `search` = `0912113555` (từ khóa tìm kiếm chung: tên/email/phone)
 - `page` = `1`
 - `pageSize` = `10`
 - `sortBy` = (ví dụ `customer_full_name` hoặc để trống)
 - `sortDirection` = `asc` / `desc` hoặc để trống
- Headers:
 - `Accept: application/json`
- Sample curl:
 ```bash
 curl "{{baseUrl}}/api/customer/paging?search=0912113555&page=1&pageSize=10" -H "Accept: application/json"
 ```

###2) GET — Lấy chi tiết khách hàng
- Name: `Customers - Get By Id`
- Method: GET
- URL: `{{baseUrl}}/api/customer/{id}`
- Replace `{id}` bằng GUID của khách hàng
- Headers: `Accept: application/json`

###3) POST — Tạo khách hàng
- Name: `Customers - Create`
- Method: POST
- URL: `{{baseUrl}}/api/customer`
- Headers: `Content-Type: application/json`
- Body (raw JSON):
 ```json
 {
 "customerFullName": "Nguyễn Văn A",
 "customerPhone": "0912345678",
 "customerEmail": "a@example.com",
 "customerType": "VIP",
 "customerShippingAddr": "123 Phố Huế",
 "customerTaxCode": "0123456789"
 }
 ```

###4) PUT — Cập nhật khách hàng
- Name: `Customers - Update`
- Method: PUT
- URL: `{{baseUrl}}/api/customer/{id}`
- Headers: `Content-Type: application/json`
- Body: các field cần cập nhật (JSON)

###5) DELETE — Xóa mềm
- Name: `Customers - Delete`
- Method: DELETE
- URL: `{{baseUrl}}/api/customer/{id}`
- Response:204 No Content hoặc ApiResponse

###6) POST — Import CSV (upload file)
- Name: `Customers - Import CSV`
- Method: POST
- URL: `{{baseUrl}}/api/customer/import`
- Body: `form-data`
 - Key: `file` — Type: File — Value: chọn file CSV (ví dụ `customers_seed.csv`)
- Notes: Server trả `ImportResult` JSON gồm `TotalRows`, `SuccessCount`, `ErrorCount`, `Errors[]`.

###7) GET — Export selected customers -> CSV
- Name: `Customers - Export CSV`
- Method: GET
- URL: `{{baseUrl}}/api/customer/export?ids={id1,id2,...}`
- Response: file/csv (download)

---

## Upload/Update khách hàng kèm file (API `with-file`) — đồng bộ với controller

Controller có các endpoint hỗ trợ tạo/cập nhật khách hàng cùng file (avatar) sử dụng suffix `with-file`. Thêm phần này vào README để đồng bộ với `CustomerController`.

### POST — Tạo khách hàng kèm file
- Endpoint:
 `POST {{baseUrl}}/api/customer/with-file`
- Content-Type: `multipart/form-data`
- Body (form-data):
 - Text fields: các thuộc tính của `Customer` (ví dụ `CustomerFullName`, `CustomerPhone`, `CustomerEmail`, `CustomerType`, `CustomerShippingAddr`, `CustomerTaxCode`, ...)
 - File field: key `avatar` (Type: File) — file ảnh (jpg/png)
- Ví dụ Postman (form-data):
 - `CustomerFullName` = `Nguyễn Văn A`
 - `CustomerPhone` = `0912345678`
 - `avatar` = (file)
- Curl example:
```bash
curl -X POST "{{baseUrl}}/api/customer/with-file" \
 -H "Accept: application/json" \
 -F "CustomerFullName=Nguyễn Văn A" \
 -F "CustomerPhone=0912345678" \
 -F "avatar=@/path/to/avatar.jpg"
```
- Response: `201 Created` (ApiResponse chứa kết quả, controller hiện trả `ApiResponse<int>.Created`).

### PUT — Cập nhật khách hàng kèm file
- Endpoint:
 `PUT {{baseUrl}}/api/customer/{id}/with-file`
- Content-Type: `multipart/form-data`
- Body (form-data):
 - Text fields: các thuộc tính muốn cập nhật
 - File field: key `avatar` (Type: File) — file ảnh mới (tuỳ chọn)
- Curl example:
```bash
curl -X PUT "{{baseUrl}}/api/customer/{id}/with-file" \
 -H "Accept: application/json" \
 -F "CustomerFullName=Nguyễn Văn B" \
 -F "avatar=@/path/to/new-avatar.jpg"
```
- Behavior: nếu upload avatar mới, server lưu file mới và xóa file cũ (nếu khác); nếu không upload file, avatar giữ nguyên.

### SEED DATA (Development environment)
- File mẫu: `customers_seed.csv` trong thư mục gốc project API.
- // Seed data CSV file when running in Development only
if (app.Environment.IsDevelopment())
{
    try
    {
        var seedPath = Path.Combine(app.Environment.ContentRootPath, "customers_seed3.csv");
        // Generate2000 records by default (adjust as needed)
        CustomerSeeder.GenerateCsv(1000, seedPath);
        Console.WriteLine($"Customer seed CSV generated: {seedPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to generate seed CSV: {ex.Message}");
    }

    // If SEED_ONLY env var is set to1, exit after seeding (one-off run)
    var seedOnly = Environment.GetEnvironmentVariable("SEED_ONLY");
    if (!string.IsNullOrEmpty(seedOnly) && seedOnly == "1")
    {
        Console.WriteLine("SEED_ONLY=1 detected, exiting after seeding.");
        return;
    }
}



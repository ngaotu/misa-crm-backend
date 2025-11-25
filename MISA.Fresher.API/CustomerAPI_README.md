# Customer API Documentation

## Overview
The Customer API provides comprehensive CRUD operations for managing customers with advanced features like pagination, sorting, filtering, and search capabilities.

## Features

###1. Basic CRUD Operations
- **Create**: `POST /api/customer` - Create a new customer
- **Read**: `GET /api/customer/{id}` - Get customer by ID
- **Update**: `PUT /api/customer/{id}` - Update existing customer
- **Delete**: `DELETE /api/customer/{id}` - Soft delete customer

###2. Advanced Querying
- **Pagination**: Support for page and pageSize parameters
- **Sorting**: Sort by any customer field (CustomerFullName, CustomerEmail, etc.)
- **Filtering**: Filter by specific customer properties
- **Search**: Quick search across multiple fields (name, email, phone, code)

###3. API Endpoints

#### Get Customers (with pagination and filtering)
```
GET /api/customer?page=1&pageSize=10&sortBy=CustomerFullName&sortDirection=asc&search=keyword
```

**Query Parameters:**
- `page` (int): Page number (default:1)
- `pageSize` (int): Number of records per page (default:10, max:100)
- `sortBy` (string): Field to sort by (CustomerCode, CustomerFullName, CustomerEmail, etc.)
- `sortDirection` (string): Sort direction - "asc" or "desc"
- `search` (string): Quick search keyword
- `customerName` (string): Filter by customer name
- `customerEmail` (string): Filter by customer email
- `customerPhone` (string): Filter by customer phone
- `customerTypeId` (guid): Filter by customer type
- `isDeleted` (bool): Filter by deletion status

**Response Format:**
```json
{
 "success": true,
 "data": {
 "items": [...],
 "totalRecords":100,
 "currentPage":1,
 "pageSize":10,
 "totalPages":10,
 "hasPrevious": false,
 "hasNext": true
 },
 "message": "L?y danh sách khách hàng thành công"
}
```

#### Create Customer
```
POST /api/customer
Content-Type: application/json
```

**Request Body:**
```json
{
 "customerCode": "", // Auto-generated if empty
 "customerFullName": "John Doe", // Required
 "customerTaxCode": "123456789",
 "customerEmail": "john@example.com", // Must be unique
 "customerPhone": "0987654321", // Must be unique
 "customerTypeId": "guid", // Required
 "customerShippingAddr": "123 Main St",
 "customerLastPurchaseDate": "2024-01-15"
}
```

#### Update Customer
```
PUT /api/customer/{id}
Content-Type: application/json
```

#### Search Customers
```
GET /api/customer/search?searchTerm=keyword
```

###4. Validation Rules
- **CustomerCode**: Auto-generated with format KHyyyyMMxxxxxx if not provided
- **CustomerFullName**: Required, max length128
- **CustomerEmail**: Must be valid email format, unique across system
- **CustomerPhone**: Must be unique across system
- **CustomerTypeId**: Required, must be valid GUID

###5. Error Handling
All endpoints return consistent error responses:
```json
{
 "success": false,
 "message": "Error description",
 "errors": ["Detailed error1", "Detailed error2"]
}
```

**Common HTTP Status Codes:**
-200: Success
-201: Created
-400: Bad Request (validation errors)
-404: Not Found
-409: Conflict (unique constraint violations)
-500: Internal Server Error

###6. Database Schema Mapping
The API uses DTOs to separate the API contract from the database schema:
- Customer entity contains all database fields
- CustomerDTO exposes only relevant API fields
- Automatic mapping between Entity ? DTO

###7. Soft Delete
Customers are soft-deleted (marked as deleted but not removed from database):
- `DELETE /api/customer/{id}` sets `isDeleted = true`
- Deleted customers are excluded from normal queries unless explicitly requested

## File endpoints (upload / download) — ?úng theo controller ("with-file")

Các endpoint upload/download file c?a controller dùng suffix `with-file` nh? sau. S? d?ng `multipart/form-data` và truy?n các tr??ng c?a `Customer` d??i d?ng form fields.

###1) T?o khách hàng kèm file (avatar)
- Endpoint:
```
POST /api/customer/with-file
```
- Mô t?: T?o khách hàng và upload avatar cùng lúc. Request ph?i là `multipart/form-data`.
- Body (form-data):
 - Fields d?ng text cho các thu?c tính Customer (ví d? `CustomerFullName`, `CustomerPhone`, `CustomerEmail`, `CustomerType`, `CustomerShippingAddr`, `CustomerTaxCode`, ...). Tên field ph?i kh?p property name c?a `Customer` model.
 - Key: `avatar` — Type: File — ch?n file ?nh (jpg/png)
- Ví d? Postman (form-data):
 - Key: `CustomerFullName` -> Value: `Nguy?n V?n A`
 - Key: `CustomerPhone` -> Value: `0912345678`
 - Key: `avatar` -> File: `avatar.jpg`
- Curl example:
```bash
curl -X POST "{{baseUrl}}/api/customer/with-file" \
 -H "Accept: application/json" \
 -F "CustomerFullName=Nguy?n V?n A" \
 -F "CustomerPhone=0912345678" \
 -F "avatar=@/path/to/avatar.jpg"
```
- Response: `201 Created` v?i ApiResponse ch?a giá tr? tr? v? (ví d? id ho?c s? b?n ghi). Controller hi?n tr? `ApiResponse<int>.Created`.

###2) C?p nh?t khách hàng kèm file (avatar)
- Endpoint:
```
PUT /api/customer/{id}/with-file
```
- Mô t?: C?p nh?t thông tin khách hàng và (tùy ch?n) upload avatar m?i. N?u upload avatar m?i, file c? s? b? xóa.
- Body (form-data):
 - Text fields cho các thu?c tính Customer (same as create)
 - Key: `avatar` — Type: File — file ?nh m?i (n?u mu?n)
- Curl example:
```bash
curl -X PUT "{{baseUrl}}/api/customer/{id}/with-file" \
 -H "Accept: application/json" \
 -F "CustomerFullName=Nguy?n V?n B" \
 -F "avatar=@/path/to/new-avatar.jpg"
```
- Response: `200 OK` v?i ApiResponse ch?a s? b?n ghi b? ?nh h??ng

###3) Import CSV (existing endpoint)
- Endpoint:
```
POST /api/customer/import
```
- Body: `form-data`
 - Key: `file` — Type: File — ch?n file CSV
- Notes: Server th?c hi?n streaming import (read ? validate ? insert). Tr? `ImportResult` (TotalRows, SuccessCount, ErrorCount, Errors[])

###4) Export CSV (existing endpoint)
- Endpoint:
```
POST /api/customer/export
```
- Body: JSON array of GUIDs in request body
- Response: file CSV download

---

## Usage Examples
See `CustomerAPI.http` file for detailed request examples.
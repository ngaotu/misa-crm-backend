# Customer API Documentation

## Overview
The Customer API provides comprehensive CRUD operations for managing customers with advanced features like pagination, sorting, filtering, and search capabilities.

## Features

### 1. Basic CRUD Operations
- **Create**: `POST /api/customer` - Create a new customer
- **Read**: `GET /api/customer/{id}` - Get customer by ID
- **Update**: `PUT /api/customer/{id}` - Update existing customer
- **Delete**: `DELETE /api/customer/{id}` - Soft delete customer

### 2. Advanced Querying
- **Pagination**: Support for page and pageSize parameters
- **Sorting**: Sort by any customer field (CustomerFullName, CustomerEmail, etc.)
- **Filtering**: Filter by specific customer properties
- **Search**: Quick search across multiple fields (name, email, phone, code)

### 3. API Endpoints

#### Get Customers (with pagination and filtering)
```
GET /api/customer?page=1&pageSize=10&sortBy=CustomerFullName&sortDirection=asc&search=keyword
```

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Number of records per page (default: 10, max: 100)
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
 "totalRecords": 100,
    "currentPage": 1,
    "pageSize": 10,
  "totalPages": 10,
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

### 4. Validation Rules
- **CustomerCode**: Auto-generated with format KHyyyyMMxxxxxx if not provided
- **CustomerFullName**: Required, max length 128
- **CustomerEmail**: Must be valid email format, unique across system
- **CustomerPhone**: Must be unique across system
- **CustomerTypeId**: Required, must be valid GUID

### 5. Error Handling
All endpoints return consistent error responses:
```json
{
  "success": false,
  "message": "Error description",
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

**Common HTTP Status Codes:**
- 200: Success
- 201: Created
- 400: Bad Request (validation errors)
- 404: Not Found
- 409: Conflict (unique constraint violations)
- 500: Internal Server Error

### 6. Database Schema Mapping
The API uses DTOs to separate the API contract from the database schema:
- Customer entity contains all database fields
- CustomerDTO exposes only relevant API fields
- Automatic mapping between Entity ? DTO

### 7. Soft Delete
Customers are soft-deleted (marked as deleted but not removed from database):
- `DELETE /api/customer/{id}` sets `isDeleted = true`
- Deleted customers are excluded from normal queries unless explicitly requested

## Usage Examples
See `CustomerAPI.http` file for detailed request examples.
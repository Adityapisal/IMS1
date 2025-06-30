# Inventory Management System

A web-based Product Inventory Management System built with ASP.NET Core MVC and Entity Framework Core.

## Features

- Category Management
- Product Management with CRUD Operations
- Filtering and Searching Products
- Pagination
- Low Quantity Indicators
- Excel Export Functionality
- Image Upload for Products

## Technologies Used

- ASP.NET Core 8.0
- Entity Framework Core
- SQLite Database
- Bootstrap 5
- Bootstrap Icons
- EPPlus for Excel Export

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later

### Installation

1. Clone the repository
```
git clone https://github.com/yourusername/inventory-management-system.git
cd inventory-management-system
```

2. Build and run the application
```
dotnet build
dotnet run
```

3. Navigate to `https://localhost:5001` or `http://localhost:5000` in your web browser

## Database

The application uses SQLite as the database provider. The database file (`inventory.db`) will be created automatically when you run the application for the first time.

## Deployment

This application can be deployed to any platform that supports ASP.NET Core, such as:
- Azure App Service
- Render
- Heroku
- AWS Elastic Beanstalk

## License

This project is licensed under the MIT License - see the LICENSE file for details. 
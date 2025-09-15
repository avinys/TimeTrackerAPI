# TimeTrackerAPI

Backend API for the TimeTracker application.
Provides authentication, project tracking, time logging, mail SMTP support.

## Technologies
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core
- MySQL database
- JWT Authentication
- BCrypt for password hashing
- Zoho Mail SMTP for emails

## Features
- User registration & login
- Email confirmation and password reset with expiring tokens
- Google OAuth login
- Protected routes with JWT (HttpOnly cookies)
- Project & time tracking endpoints

## Setup

### Prerequisites
- .NET 8 SDK
- SQL Server, mySQL or localdb (set the connection string accordingly)
- Entity Framework Core

### 1. Clone repo
`git clone https://github.com/avinys/TimeTrackerAPI.git`

`cd TimeTrackerAPI`

### 2. Configure secrets

"JwtSettings:SecretKey": random-secret-key

"Email:Smtp:Username": no-reply email

"Email:Smtp:Port": "587"

"Email:Smtp:Password": zoho-app-password

"Email:Smtp:Host": "smtp.zoho.eu"

"Email:Smtp:FromAddress": no-reply email

"ConnectionStrings:DefaultConnection": database connection string

"Authentication:Google:ClientId": Google OAuth client id

"App:PublicUrl": http://localhost:5173 or app url

"App:ContactEmail": email for contact

"Email:Smtp:FromName": "TimeTracker"

### 3. Run migrations
`dotnet ef database update`

### 4. Run the API
`dotnet run`

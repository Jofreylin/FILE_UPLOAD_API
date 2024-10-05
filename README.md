
# File Upload API

This is a .NET 7-based File Upload API, designed with the repository pattern and abstract factory to support various upload services.

## Features

- **Multiple File Storage Options**: Save files to a local server, AWS S3, or an FTP server.
- **Entity Framework Integration**: Store file metadata in a SQL Server database using Entity Framework.
- **Database-First Approach**: The project uses a database-first design, with scaffolding to generate the necessary models and context.
- **SQL Backup**: The repository includes a SQL Server database backup named `backup database.sql`.

## Technologies Used

- .NET 7
- Entity Framework Core
- AWS S3
- FTP Server
- SQL Server (Minimum version: SQL Server 2019)
- Repository Pattern
- Abstract Factory Pattern

## Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/Jofreylin/FILE_UPLOAD_API.git
   ```

2. Restore the dependencies:

   ```bash
   dotnet restore
   ```

3. Set up the SQL Server:

   - You must have **SQL Server 2019** or later.
   - Execute the SQL script `backup database.sql` located in the root folder to restore the database.

4. Scaffolding:

   This project uses a **database-first** approach. If you make changes to the database schema, use the following scaffolding command to update the models:

   ```bash
   dotnet ef dbcontext scaffold Name=ConnectionStrings:DBConnection Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --force --data-annotations --project ./FILE_UPLOAD_API --context "DocApiContext"
   ```

5. Update the connection string and storage configurations in `appsettings.json`:

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "ConnectionStrings": {
       "DBCONNECTION": "Data Source=.;Initial Catalog=DOCU;Integrated Security=True;Trust Server Certificate=true"
     },
     "StorageConfigurations": {
       "S3": {
         "Region": "us-west-2",
         "AccessKey": "accesskey99",
         "SecretKey": "secretkey97",
         "Bucket": "name-bucket",
         "ServiceURL": "https://ewr1.vultrobjects.com/"
       },
       "FTP": {
         "Host": "ftp://yourftphost.com",
         "UrlHost": "https://yourftphost.com",
         "Port": 990,
         "User": "usermname",
         "Pass": "password"
       }
     },
     "ApiBaseUrl": "https://localhost:7207",
     "AllowedHosts": "*"
   }
   ```

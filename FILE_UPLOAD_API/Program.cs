using FILE_UPLOAD_API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using FILE_UPLOAD_API.Context;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using FILE_UPLOAD_API.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var s3Configuration = builder.Configuration.GetSection("StorageConfigurations:S3");
var ftpConfiguration = builder.Configuration.GetSection("StorageConfigurations:FTP");

builder.Services.Configure<S3Credentials>(s3Configuration);
builder.Services.Configure<FTPCredentials>(ftpConfiguration);

builder.Services.AddDbContext<DocApiContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DBCONNECTION")));

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<S3StorageService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<FTPStorageService>();
builder.Services.AddScoped<StorageServiceFactory>();
builder.Services.AddScoped<IDocumentRepository, DocumentService>();
builder.Services.AddScoped<ILogRepository, LogService>();



var app = builder.Build();

app.UseCors(x => x
                 .SetIsOriginAllowed(origin => true)
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
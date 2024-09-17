using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyAPI.Models;
using MyAPI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DBMonggoSetting>(builder.Configuration.GetSection("DBMonggoSetting"));
builder.Services.AddSingleton<IDBMonggoSetting>(sp => sp.GetRequiredService<IOptions<DBMonggoSetting>>().Value);
builder.Services.AddSingleton<IMongoClient>(e =>
{
    var connectionString = builder.Configuration.GetSection("DBMonggoSetting:ConnectionString").Value;
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton<UserServices>();
builder.Services.AddSingleton<AccountServices>();
builder.Services.AddScoped<ImageServices>();
builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.")))
    };
});



// Thêm cấu hình từ appsettings.json
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Thêm Cloudinary vào DI container
builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var cloudinarySettings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new CloudinaryDotNet.Account(
        cloudinarySettings.CloudName,
        cloudinarySettings.ApiKey,
        cloudinarySettings.ApiSecret));
});



builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5047);
});

var app = builder.Build();
app.Urls.Add("http://*:" + Environment.GetEnvironmentVariable("PORT") ?? "5047");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
app.UseSwagger(); 
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();



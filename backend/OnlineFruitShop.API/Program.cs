using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;
using OnlineFruitShop.Infrastructure.Data;
using OnlineFruitShop.Infrastructure.Repositories;
using OnlineFruitShop.Infrastructure.Services;
using AutoMapper;
using System.Text;
using OnlineFruitShop.API.Models;

var builder = WebApplication.CreateBuilder(args);
// Ensure the backend listens on http://localhost:5000 during development
// Ensure the backend listens on both HTTP and HTTPS for local development
builder.WebHost.UseUrls("http://localhost:5000;https://localhost:5001");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IDeliveryReceiptService, DeliveryReceiptService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("DefaultCors");
// Disable HTTPS redirection in local development so API calls from the frontend
// keep the Authorization header when using http://localhost:5000.
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    db.Database.Migrate();

    const string adminEmail = "----adminmail-----";
    var existingAdmin = db.Users.FirstOrDefault(u => u.Role == "Admin");
    if (existingAdmin == null)
    {
        var adminPassword = "----password--";
        var hashedPassword = authService.HashPasswordAsync(adminPassword).GetAwaiter().GetResult();
        var adminUser = new User
        {
            Name = "Administrator",
            Email = adminEmail,
            PasswordHash = hashedPassword,
            Role = "Admin"
        };
        db.Users.Add(adminUser);
        db.SaveChanges();
    }
    else if (existingAdmin.Email != adminEmail)
    {
        existingAdmin.Email = adminEmail;
        db.SaveChanges();
    }
}

app.MapControllers();

app.Run();

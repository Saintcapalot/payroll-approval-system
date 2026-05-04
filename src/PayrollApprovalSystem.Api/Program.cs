using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PayrollApprovalSystem.Api.Extensions;
using PayrollApprovalSystem.Infrastructure.Extensions;
using PayrollApprovalSystem.Infrastructure.Persistence;
using QuestPDF.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Payroll Approval System API");

    var builder = WebApplication.CreateBuilder(args);

    QuestPDF.Settings.License = LicenseType.Community;

    builder.Host.UseSerilog((context, services, loggerConfig) =>
        loggerConfig.ReadFrom.Configuration(context.Configuration));

    // API services
    builder.Services.AddControllers();
    builder.Services.AddSwaggerDocumentation();
    builder.Services.AddApiServices();

    //  CORS (for demo.html)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowDemoClient", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Infrastructure (PostgreSQL + repositories)
    builder.Services.AddInfrastructure(
        builder.Configuration.GetConnectionString("DefaultConnection")!);

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JwtSettings:Key is missing.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Auto-migrate database
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();

        if (app.Environment.IsDevelopment())
            await DatabaseSeeder.SeedDevelopmentDataAsync(dbContext);
    }

    // Configure pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    //  Enable CORS
    app.UseCors("AllowDemoClient");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
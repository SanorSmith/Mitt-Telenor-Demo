using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using BillingService.Data;
using BillingService.Services;
using Shared.Middleware;
using Shared.Messaging;
using Amazon.SimpleNotificationService;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Billing Service API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new()
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new()
        {
            {
                new()
                {
                    Reference = new()
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddDbContext<BillingDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    var snsConfig = new AmazonSimpleNotificationServiceConfig
    {
        ServiceURL = builder.Configuration["AWS:ServiceURL"]
    };
    builder.Services.AddSingleton<IAmazonSimpleNotificationService>(
        new AmazonSimpleNotificationServiceClient("test", "test", snsConfig));
    builder.Services.AddSingleton<IEventPublisher, SnsEventPublisher>();

    builder.Services.AddScoped<IBillingService, BillingService.Services.BillingService>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
        db.Database.Migrate();
    }

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Billing Service starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Billing Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}

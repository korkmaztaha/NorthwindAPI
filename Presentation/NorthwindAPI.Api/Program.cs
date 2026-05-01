using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NorthwindApi.Application;
using NorthwindApi.Persistence;
using NorthwindAPI.Api.Middleware;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var columnOptions = new ColumnOptions();


columnOptions.Store.Remove(StandardColumn.MessageTemplate);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("NorthwindConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        },
        columnOptions: columnOptions) 
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddRateLimiter(options =>
{
    // Rate limit aşıldığında dönecek HTTP status code
    options.RejectionStatusCode = 429;

    // Genel endpoint'ler için limit (dakikada 60 istek)
    options.AddSlidingWindowLimiter("GeneralPolicy", opt =>
    {
        opt.PermitLimit = 60; // 1 dakikada max 60 istek
        opt.Window = TimeSpan.FromMinutes(1); // toplam süre: 1 dakika
        opt.SegmentsPerWindow = 6; // pencereyi 6 parçaya böler (her biri 10 sn)
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // eski istekler önce işlenir
        opt.QueueLimit = 0; // kuyruğa alma yok, limit aşılırsa direkt reddet
    });

    // Rapor endpoint'leri için limit (dakikada 10 istek)
    options.AddFixedWindowLimiter("ReportPolicy", opt =>
    {
        opt.PermitLimit = 10; // 1 dakikada max 10 istek
        opt.Window = TimeSpan.FromMinutes(1); // sabit zaman penceresi
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // kuyruk yok
    });

    // 3️⃣ Rapor endpoint'leri için eş zamanlı istek sınırı
    options.AddConcurrencyLimiter("ReportConcurrencyPolicy", opt =>
    {
        opt.PermitLimit = 5; // aynı anda max 5 istek işlenebilir
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // fazlası direkt reddedilir
    });

    // 4️⃣ Auth endpoint'leri (login vs.) için brute-force koruması
    options.AddFixedWindowLimiter("AuthPolicy", opt =>
    {
        opt.PermitLimit = 5; // 1 dakikada max 5 deneme
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            StatusCode = 429,
            Message = "Çok fazla istek gönderdiniz. Lütfen bekleyin.",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);
    };
});

var app = builder.Build();
app.UseRateLimiter();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


try
{
    Log.Information("Uygulama başlatılıyor...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama başlatılamadı!");
}
finally
{
    Log.CloseAndFlush();
}

app.Run();
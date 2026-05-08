using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NorthwindApi.Application;
using NorthwindApi.Persistence;
using NorthwindApi.Persistence.Jobs;
using NorthwindAPI.Api.Filters;
using NorthwindAPI.Api.Middleware;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;
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
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration["Hangfire:ConnectionString"]));

builder.Services.AddHangfireServer();

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Northwind API",
        Version = "v1",
        Description = "Northwind veritabanı üzerine inşa edilmiş Clean Architecture API",
        Contact = new OpenApiContact
        {
            Name = "Northwind API",
            Email = "info@northwindapi.com"
        }
    });

    // XML yorum dosyası
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Annotation desteği
    c.EnableAnnotations();

    // JWT
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
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("NorthwindConnection")!,
        name: "sql-server",
        tags: new[] { "db", "sql" })
    .AddRedis(
        redisConnectionString: builder.Configuration["Redis:ConnectionString"]!,
        name: "redis",
        tags: new[] { "cache", "redis" });


builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(30); // 30 saniyede bir kontrol
    opt.AddHealthCheckEndpoint("NorthwindApi", "/health");
})
.AddInMemoryStorage();

var app = builder.Build();
app.UseRateLimiter();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter(app.Environment) }
});
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    // Her 30 saniyede bir outbox mesajlarını işle
    recurringJobManager.AddOrUpdate<OutboxProcessor>(
        "outbox-processor",
        job => job.ProcessAsync(),
        "*/30 * * * * *");
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TokenBlacklistMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

// Health Check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/cache", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("cache"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

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


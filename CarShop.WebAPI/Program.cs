using DataAccessLayer.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BusinessLayer.Conteiner;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using CarShop.WebAPI.Mapping;
using BusinessLayer.RabbitMQ;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text; // Redis için gerekli using eklendi

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL bağlantı dizesini al
var postgreSqlConnectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");

// PostgreSQL DbContext'i servislere ekle
builder.Services.AddDbContext<CarShopContext>(options =>
    options.UseNpgsql(postgreSqlConnectionString));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// FluentValidation için doğrulayıcıları derlemeden ekle
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// EnhancedRabbitMQService'i Singleton olarak ekle
builder.Services.AddSingleton<EnhancedRabbitMQService>();

// Redis bağlantı dizesini al
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// Redis bağlantı dizesi null veya boşsa hata fırlat
if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new InvalidOperationException("Redis bağlantı dizesi bulunamadı. Lütfen appsettings.json dosyasını kontrol edin.");
}

// Redis'i ekle
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

// JWT Ayarları
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
// StackExchange.Redis.IConnectionMultiplexer'ı DI konteynerına Singleton olarak kaydet
// Bu, uygulamanın yaşam döngüsü boyunca tek bir Redis bağlantısı sağlar.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    // Redis bağlantı seçeneklerini yapılandır
    var options = ConfigurationOptions.Parse(redisConnectionString);
    options.AbortOnConnectFail = false; // Bağlantı hatasında hemen başarısız olma
    options.SyncTimeout = 5000; // Senkron işlemler için timeout süresi
    options.Password = builder.Configuration.GetConnectionString("RedisPassword"); // Redis şifresini al ve ayarla

    return ConnectionMultiplexer.Connect(options);
});


// BusinessLayer'daki bağımlılıkları eklemek için genişletme metodunu çağır
builder.Services.ConteinerDependencies();

// Kontrolcüleri ve JSON seçeneklerini ekle (ReferenceHandler.Preserve ile döngüsel referansları yönet)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// Identity hizmetlerini yapılandır ve veritabanı mağazasını ekle
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Şifre gereksinimlerini ayarla
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // E-posta benzersizliğini zorunlu kıl
    options.User.RequireUniqueEmail = true;

    // E-posta ve hesap onayını şimdilik devre dışı bırak
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CarShopContext>() // DbContext'i Identity için depo olarak kullan
.AddDefaultTokenProviders(); // Şifre sıfırlama, e-posta onayı gibi token sağlayıcıları ekle

// AutoMapper'ı GeneralMapping sınıfını kullanarak ekle
builder.Services.AddAutoMapper(typeof(GeneralMapping));

// Konsol çıktısı için loglama hizmetlerini ekle
builder.Services.AddLogging(builder => builder.AddConsole());

// API keşfi için uç nokta API gezgini ekle
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI desteğini ekle
builder.Services.AddSwaggerGen();

// Uygulamayı oluştur
var app = builder.Build();

// Geliştirme ortamında Swagger UI'ı etkinleştir
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS yönlendirmesini kullan
app.UseHttpsRedirection();

// Statik dosyaları sunmayı etkinleştir (wwwroot klasörü gibi)
app.UseStaticFiles();

// Yetkilendirme middleware'ini kullan
app.UseAuthorization();

// Kontrolcü uç noktalarını eşle
app.MapControllers();

// Uygulama başlatıldığında roller ve admin kullanıcı oluşturmak için scope oluştur
using (var scope = app.Services.CreateScope())
{
    // Gerekli servisleri al
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var rabbitMqService = scope.ServiceProvider.GetRequiredService<EnhancedRabbitMQService>();

    // RabbitMQ kuyruklarını ayarla
    rabbitMqService.SetupAllEntityQueues();

    // Yönetici rolü ve kullanıcısı için varsayılan bilgiler
    string adminRole = "Admin";
    string adminEmail = "kcdmirapo96@gmail.com";
    string adminPassword = "123456aA*";

    // Admin rolü yoksa oluştur
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Name = adminRole,
            Description = "Sistem Yöneticisi"
        });
    }

    // Admin kullanıcısı yoksa oluştur ve role ata
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = "Apo2550",
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Abdullah",
            LastName = "KOÇDEMİR",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("✅ Admin kullanıcısı oluşturuldu!");
        }
        else
        {
            // Kullanıcı oluşturma hatalarını konsola yazdır
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"❌ Admin Kullanıcı Oluşturma Hatası: {error.Description}");
            }
        }
    }
}

// Uygulamanın başlatıldığını konsola yazdır
Console.WriteLine("🚀 CarShop Web API başlatıldı!");

// Uygulamayı çalıştır
app.Run();

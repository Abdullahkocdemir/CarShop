using BusinessLayer.RabbitMQ;
using CarShop.WebUI.Mapping;
using CarShop.WebUI.Models; // Ensure this namespace is correctly imported for ApiSettings
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure ApiSettings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Add services for MVC controllers and views
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("CarShopApiClient", (sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    if (string.IsNullOrEmpty(apiSettings.BaseUrl))
    {
        throw new InvalidOperationException("API BaseUrl is not configured in appsettings.json or is empty.");
    }
    client.BaseAddress = new Uri(apiSettings.BaseUrl);

});

// Add FluentValidation for model validation
builder.Services.AddValidatorsFromAssemblyContaining<BusinessLayer.ValidationRules.BrandValidation.CreateBrandDTOValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Add AutoMapper for object mapping
builder.Services.AddAutoMapper(typeof(GeneralMapping));

// Configure and add Session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout (e.g., 30 minutes)
    options.Cookie.HttpOnly = true; // Make the session cookie inaccessible to client-side script
    options.Cookie.IsEssential = true; // Make the session cookie essential for the app to function
});

// Register RabbitMQ Consumer Service as a Singleton
builder.Services.AddSingleton<EnhancedRabbitMQConsumerService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    // Retrieve RabbitMQ settings with default fallback values
    var hostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
    var userName = configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest"; // Common default
    var password = configuration.GetValue<string>("RabbitMQ:Password") ?? "guest"; // Common default

    // It's important to provide actual RabbitMQ credentials if they differ from defaults.
    // The user provided "admin" and "Abdullah159" so I will use those.
    hostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
    userName = configuration.GetValue<string>("RabbitMQ:UserName") ?? "admin";
    password = configuration.GetValue<string>("RabbitMQ:Password") ?? "Abdullah159";

    var consumerService = new EnhancedRabbitMQConsumerService(hostName, userName, password);
    consumerService.StartConsumingAllEntities(); // Start consuming messages immediately
    return consumerService;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files (e.g., CSS, JS, images)

app.UseRouting(); // Enables endpoint routing

// IMPORTANT: UseSession must be placed after UseRouting and before UseAuthorization/UseEndpoints
app.UseSession(); // Enables session state for the application

app.UseAuthorization(); // Enables authorization middleware

// Maps incoming requests to controller actions
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("🚀 CarShop Web UI başlatıldı ve tüm RabbitMQ mesajları dinleniyor!");
app.Run();

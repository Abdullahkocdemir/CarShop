using BusinessLayer.RabbitMQ;
using CarShop.WebUI.Mapping;
using CarShop.WebUI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using NuGet.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
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

// 2. Belirli bir assembly'deki t�m AbstractValidator'lar� otomatik olarak kaydet
// 'BusinessLayer' projenizin ad�n� buraya yaz�n.
// Bu, o assembly i�indeki t�m 'AbstractValidator<T>' s�n�flar�n� bulup kaydeder.
builder.Services.AddValidatorsFromAssemblyContaining<BusinessLayer.ValidationRules.BrandValidation.CreateBrandDTOValidator>();
builder.Services.AddFluentValidationAutoValidation(); // Do�rulamay� otomatik yapar
builder.Services.AddFluentValidationClientsideAdapters(); // Client-side (taray�c� taraf�) do�rulamay� etkinle�tirir
// AutoMapper'� kaydetme
builder.Services.AddAutoMapper(typeof(GeneralMapping)); 

builder.Services.AddSingleton<EnhancedRabbitMQConsumerService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var hostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
    var userName = configuration.GetValue<string>("RabbitMQ:UserName") ?? "admin";
    var password = configuration.GetValue<string>("RabbitMQ:Password") ?? "Abdullah159";

    var consumerService = new EnhancedRabbitMQConsumerService(hostName, userName, password);
    consumerService.StartConsumingAllEntities(); 
    return consumerService;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Console.WriteLine("?? CarShop Web UI ba�lat�ld� ve t�m RabbitMQ mesajlar� dinleniyor!");
app.Run();

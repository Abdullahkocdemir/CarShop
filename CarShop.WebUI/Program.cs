using BusinessLayer.RabbitMQ;
using CarShop.WebUI.Models;
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

Console.WriteLine("?? CarShop Web UI baþlatýldý ve tüm RabbitMQ mesajlarý dinleniyor!");
app.Run();

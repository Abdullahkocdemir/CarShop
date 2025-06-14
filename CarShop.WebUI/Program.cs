using BusinessLayer.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ?? YEN� ENHANCED RABBITMQ CONSUMER SERV�S�
builder.Services.AddSingleton<EnhancedRabbitMQConsumerService>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var hostName = configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost";
    var userName = configuration.GetValue<string>("RabbitMQ:UserName") ?? "admin";
    var password = configuration.GetValue<string>("RabbitMQ:Password") ?? "Abdullah159";

    var consumerService = new EnhancedRabbitMQConsumerService(hostName, userName, password);
    consumerService.StartConsumingAllEntities(); // ?? T�M ENT�TY'LER� D�NLE
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

using DataAccessLayer.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BusinessLayer.Conteiner;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using CarShop.WebAPI.Mapping;
using BusinessLayer.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSqlConnection");

builder.Services.AddDbContext<CarShopContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<EnhancedRabbitMQService>();

builder.Services.ConteinerDependencies();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<CarShopContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(GeneralMapping));
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    var rabbitMqService = scope.ServiceProvider.GetRequiredService<EnhancedRabbitMQService>();
    rabbitMqService.SetupAllEntityQueues();

    string adminRole = "Admin";
    string adminEmail = "kcdmirapo96@gmail.com";
    string adminPassword = "123456aA*";

    // Admin rol�n�n varl���n� kontrol et ve yoksa olu�tur
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new AppRole
        {
            Name = adminRole,
            Description = "Sistem Y�neticisi"
        });
    }

    // Admin kullan�c�s�n�n varl���n� kontrol et ve yoksa olu�tur
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = "Apo2550",
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Abdullah",
            LastName = "KO�DEM�R",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            Console.WriteLine("? Admin kullan�c�s� olu�turuldu!");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"? Admin User Creation Error: {error.Description}");
            }
        }
    }
}

Console.WriteLine("?? CarShop Web API ba�lat�ld�!");
app.Run();

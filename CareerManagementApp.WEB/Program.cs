using CareerManagementApp.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;

var builder = WebApplication.CreateBuilder(args);
var apiKey = "AIzaSyCZTwIgNQyjFQttuvVABFkLxHt-4nNHww8";
builder.Services.AddDbContext<MyDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Mysql"), b => b.MigrationsAssembly("CareerManagementApp.WEB")));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<GoogleAI>(new GoogleAI(apiKey));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

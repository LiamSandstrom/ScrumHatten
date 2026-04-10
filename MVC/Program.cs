using BL.Interfaces;
using BL.Services;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration["MongoDB:ConnectionString"];
builder.Services.AddSingleton<MongoConnector>(new MongoConnector(connectionString));


// ELin och Carro till HatRepository och HatService
builder.Services.AddScoped<IHatRepository, HatRepository>();
builder.Services.AddScoped<IHatService, HatService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

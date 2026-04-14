using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Repository;
using Repository.Repositories;
using BL.Services;
using BL.Interfaces;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration["MongoDB:ConnectionString"];
builder.Services.AddSingleton<MongoConnector>(new MongoConnector(connectionString));

<<<<<<< HEAD
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
=======
//till hatten
builder.Services.AddSingleton<HatRepository>();
builder.Services.AddScoped<IHatService, HatService>();

>>>>>>> C_E
var app = builder.Build();

#pragma warning disable CS0618
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
#pragma warning restore CS0618

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

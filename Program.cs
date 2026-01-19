using Microsoft.EntityFrameworkCore;
using HavamaGore.Data;
using HavamaGore.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<HavamaGore.Services.MoodService>();

builder.Services.AddHttpClient<SpotifyService>(); // Spotify servisini ekledik

builder.Services.AddHttpClient<BookService>();

// Veritabanı Bağlantısı (Retry/Tekrar Dene Özelliği Eklendi)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"), 
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5, 
            maxRetryDelay: TimeSpan.FromSeconds(30), 
            errorNumbersToAdd: null)
    ));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddScoped<HavamaGore.Services.WeatherService>();
builder.Services.AddScoped<HavamaGore.Services.SpotifyService>();

// --- SERVİSLERİ KAYDET (BURASI EKSİKTİ) ---
builder.Services.AddHttpClient(); // API'lara istek atmak için şart

builder.Services.AddScoped<HavamaGore.Services.WeatherService>(); // Hava durumu servisi

builder.Services.AddScoped<HavamaGore.Services.SpotifyService>(); // Müzik servisi

builder.Services.AddSession(); // Session servisini ekle

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

app.UseSession(); // Session'ı aktif et

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();



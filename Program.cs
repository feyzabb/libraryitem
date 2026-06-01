using Microsoft.EntityFrameworkCore; // KIRMIZI ÇİZGİYİ ÖNLEYEN KRİTİK SATIR

var builder = WebApplication.CreateBuilder(args);

// 🚨 VERİ TABANI BAĞLANTISINI SQLITE KULLANACAK ŞEKİLDE GÜNCELLEDİK
builder.Services.AddDbContext<Library_Item.Models.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Çerez tabanlı giriş yapma servisini projeye ekliyoruz
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriş yapmayan biri kısıtlı yere girmeye çalışırsa buraya şutlanacak
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Kullanıcı 7 gün boyunca girişli kalsın
    });
    
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // 🚨 Araya .Cookie ekleyerek hatayı çözüyoruz:
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Veri tabanındaki ilanları (gitar, keman vb.) otomatik yükleyen senin eski çalışan kodun
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    Library_Item.Models.SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 🚨 BU SATIRI EKLE (Authorization'ın hemen üstünde olmalı)
app.UseAuthorization();

app.UseSession(); // 🚨 Session'ı aktif hale getiriyoruz

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
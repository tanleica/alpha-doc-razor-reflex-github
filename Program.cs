using Microsoft.EntityFrameworkCore;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Razor
builder.Services.AddRazorPages();

// PostgreSQL
builder.Services.AddDbContext<MiukaFotoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MiukaFotoDb")));

// Services
builder.Services.AddScoped<DocxExportService>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// ❌ REMOVE SSO COMPLETELY
// builder.Services.AddAuthentication()
// builder.Services.AddMicrosoftIdentityWebApp()

var app = builder.Build();

// Error Handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Forwarded headers (keep)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});
app.UseStaticFiles();
app.UseSession();

// ❌ REMOVE THESE
// app.UseAuthentication();
// app.UseAuthorization();

// Static
app.MapStaticAssets();

// Razor Pages
app.MapRazorPages().WithStaticAssets();

app.Run();
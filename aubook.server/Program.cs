using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHsts();

    app.UseSpa(spa =>
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// app.MapStaticAssets();

// app.UseDefaultFiles();
// app.UseStaticFiles();
// app.UseStaticFiles(Path.Join(builder.Environment.ContentRootPath, "wwwroot/browser"));
// Console.WriteLine(Path.Join(builder.Environment.ContentRootPath, "wwwroot/browser"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "browser")),
    RequestPath = "",
    ServeUnknownFileTypes = true
});

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapFallbackToFile("/browser/index.html");

app.Run();

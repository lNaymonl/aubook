using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy
//             .WithOrigins("http://localhost:4200") // Angular dev server
//             .AllowAnyHeader()
//             .AllowAnyMethod();
//     });
// });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

// app.UseCors();

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

app.MapControllers();              // <-- MUST be before UseSpa / MapFallback

if (app.Environment.IsDevelopment())
{
    // Any request that got this far (i.e. NOT /api and not a static file)
    // is forwarded to the Angular dev server on :4200
    // app.UseSpa(spa =>
    // {
    //     spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    // });

    app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments("/api"), spaApp =>
    {
        spaApp.UseSpa(spa =>
        {
            spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        });
    });
}
else
{
    // Production: serve the pre-built files and fall back to index.html
    app.MapFallbackToFile("index.html");
}

app.Run();

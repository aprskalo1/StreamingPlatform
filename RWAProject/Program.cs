using Microsoft.EntityFrameworkCore;
using RWAProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RwaMoviesContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("Task06ConnStr"));
    options.UseSqlServer("server=.;Database=RwaMovies;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=Videos}/{action=Index}/{id?}");

app.Run();

using DepositosMoz.Web.Queries;
using DepositosMoz.Web.Repositories;
using DepositosMoz.Web.Data;
using Microsoft.EntityFrameworkCore;
using DepositosMoz.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ConsultasMoz>();
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IUnidadTrabajo, UnidadTrabajo>();

var proveedorBaseDatos =
    builder.Configuration["DatabaseProvider"] ?? "Sqlite";

builder.Services.AddDbContext<DepositosMozDbContext>(options =>
{
    if (proveedorBaseDatos.Equals(
        "SqlServer",
        StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DepositosMozDB"));
    }
    else
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString(
                "Sqlite"));
    }
});

var app = builder.Build();

// Crear automáticamente la base de datos.
using (var scope = app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
            .GetRequiredService<DepositosMozDbContext>();

    if (proveedorBaseDatos.Equals(
        "SqlServer",
        StringComparison.OrdinalIgnoreCase))
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        await db.Database.EnsureCreatedAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Payphone.Wallet.Infrastructure.Persistence;

namespace Payphone.Wallet.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<WalletTransferDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Registrar SQLite para las pruebas de integración
            services.AddDbContext<WalletTransferDbContext>(options =>
            {
                options.UseSqlite("Data Source=wallettransfer_test.db");
            });

            // Crear la base de datos de pruebas
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<WalletTransferDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}
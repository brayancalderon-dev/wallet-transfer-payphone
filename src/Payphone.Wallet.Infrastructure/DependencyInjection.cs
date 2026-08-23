using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payphone.Wallet.Application.Interfaces;
using Payphone.Wallet.Infrastructure.Auth;
using Payphone.Wallet.Infrastructure.Persistence;
using Payphone.Wallet.Infrastructure.Repositories;

namespace Payphone.Wallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=wallettransfer.db";

        services.AddDbContext<WalletTransferDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IMovementRepository, MovementRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<JwtTokenGenerator>();

        return services;
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace function;

public static class Handler
{
    // MapEndpoints is used to register WebApplication
    // HTTP handlers for various paths and HTTP methods.
    public static void MapEndpoints(WebApplication app)
    {
        var connectionString = File.ReadAllText("/var/openfaas/secrets/pg-connection");
        var dataSource = NpgsqlDataSource.Create(connectionString);

       app.MapGet("/", async (EmployeeDb db) =>
            await db.Employees.ToListAsync());
    }

    // MapServices can be used to configure additional
    // WebApplication services
    public static void MapServices(IServiceCollection services)
    {
        var connectionString = File.ReadAllText("/var/openfaas/secrets/pg-connection");

        services.AddDbContext<EmployeeDb>(
            optionsBuilder => optionsBuilder.UseNpgsql(connectionString)
        );
    }
}

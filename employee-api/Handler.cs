using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

        app.MapGet("/", async () =>
        {   
            var employees = new List<Employee>();
            await using (var cmd = dataSource.CreateCommand("SELECT id, name, email FROM employee"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee{
                        Id = (int)reader["id"],
                        Name = reader.GetString(1),
                        Email = reader.GetString(2)
                    });
                }
            }
            return Results.Ok(employees);
        });
    }

    // MapServices can be used to configure additional
    // WebApplication services
    public static void MapServices(IServiceCollection services)
    {
    }
}

public class Employee {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}
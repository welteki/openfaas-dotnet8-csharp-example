using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

[Table("employee")]
public class Employee
{
    [System.ComponentModel.DataAnnotations.Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")] 
    public string? Name { get; set; }

    [Column("email")] 
    public string? Email { get; set; }


}

public class EmployeeDb: DbContext
{
    public EmployeeDb(DbContextOptions<EmployeeDb> options)
        : base(options) { }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e => e.ToTable("employee"));
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");
            entity.Property(e => e.Name).IsRequired().HasColumnName("name");
            entity.Property(e => e.Email).IsRequired().HasColumnName("email");
        });

        base.OnModelCreating(modelBuilder);
    }   
}
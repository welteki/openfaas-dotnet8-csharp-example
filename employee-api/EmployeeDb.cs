using Microsoft.EntityFrameworkCore;

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

using FastLane.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        }
        public DbSet<Entities.Service> Services  { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employee  { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Detail> Order_Details { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Airport> Airports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Service>()
                .Property(u => u.Created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Entities.Service>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Role>()
                .Property(u => u.Created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Role>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Status>()
                .Property(u => u.Created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Status>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Customer>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Customer>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<User>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<User>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<UserRole>()
                .HasKey(wt => new { wt.User_Id, wt.Role_Id });
            modelBuilder.Entity<UserRole>()
                .HasOne(wt => wt.User)
                .WithMany(u => u.UserRole)
                .HasForeignKey(wt => wt.User_Id);
            modelBuilder.Entity<UserRole>()
                .HasOne(wt => wt.Role)
                .WithMany(r => r.UserRole)
                .HasForeignKey(wt => wt.Role_Id);

            modelBuilder.Entity<Employee>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Employee>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Order>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Order_Detail>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Order_Detail>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Airport>()
               .Property(u => u.Created_at)
               .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Airport>()
                .Property(u => u.Updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}

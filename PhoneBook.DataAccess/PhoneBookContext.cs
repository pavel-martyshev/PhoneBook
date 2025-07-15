using Microsoft.EntityFrameworkCore;
using PhoneBook.DataModels;

namespace PhoneBook.DataAccess;

public class PhoneBookContext(DbContextOptions<PhoneBookContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts { get; set; }

    public DbSet<PhoneNumber> PhoneNumbers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(b =>
        {
            b.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            b.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50);

            b.Property(c => c.MiddleName).HasMaxLength(50);
        });

        modelBuilder.Entity<PhoneNumber>(b =>
        {
            b.Property(p => p.Number)
                .IsRequired()
                .HasMaxLength(15);

            b.HasOne(p => p.Contact)
                .WithMany(c => c.PhoneNumbers)
                .HasForeignKey(p => p.PersonId)
                .IsRequired();
        });

        var baseType = typeof(BaseModel);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(t => baseType.IsAssignableFrom(t.ClrType)))
        {
            var entityBuilder = modelBuilder.Entity(entityType.ClrType);

            entityBuilder
                .Property(nameof(BaseModel.CreatedAt))
                .HasColumnType("datetimeoffset(0)")
                .HasDefaultValueSql("CAST(SYSDATETIMEOFFSET() AS datetimeoffset(0))");

            entityBuilder
                .Property(nameof(BaseModel.IsDeleted))
                .HasDefaultValue(false);
        }
    }
}

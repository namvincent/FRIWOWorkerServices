using FRIWOLocalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FRIWOLocalAPI.Models
{
    public class SerialPortContext : DbContext
    {
        public SerialPortContext(DbContextOptions<SerialPortContext> options)
            : base(options)
        {
        }

        public DbSet<SerialPortItem> SerialPortItems { get; set; } = null!;

        public DbSet<Unit> Units { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SerialPortItem>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable(nameof(SerialPortItem));
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable(nameof(Unit)).HasKey(e=>e.ID);
            });
        }
    }
}
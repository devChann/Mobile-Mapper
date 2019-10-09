using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace webmapper.Models
{
    public partial class NARIGPContext : DbContext
    {
        public NARIGPContext()
        {
        }
     
        public NARIGPContext(DbContextOptions<NARIGPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AndroidViewtbl> AndroidViewtbl { get; set; }
        public virtual DbSet<Parceltbl> Parceltbl { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Data Source=oslsvr\\kaspersky;Initial Catalog=NARIGP;Persist Security Info=True;User ID=swave;Password=swave;", x => x.UseNetTopologySuite());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AndroidViewtbl>(entity =>
            {
                entity.ToTable("androidViewtbl");

                entity.HasKey(k => k.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Purpose).HasColumnName("Purpose");
                entity.Property(e => e.Acreage).HasColumnName("Acreage");
                entity.Property(e => e.Polygon).HasColumnName("polygon");
                entity.Property(e => e.Polygon).HasColumnType("geometry");
            });

            modelBuilder.Entity<Parceltbl>(entity =>
            {
                entity.ToTable("Parceltbl");

                entity.HasKey(k => k.Id);

                entity.Property(e => e.Id).HasColumnName("id");

                //entity.Property(e => e.wkt).HasColumnName("wkt");

                entity.Property(e => e.Geometry).HasColumnType("geometry");
                entity.Property(e => e.jsonData).HasColumnName("jsonData");
            });
        }
    }
}

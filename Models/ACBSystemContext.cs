using Microsoft.EntityFrameworkCore;
using BecamexIDC.Pattern.EF.Factory;

namespace App.Models
{
    public partial class ACBSystemContext : DataContext
    {
        public ACBSystemContext(DbContextOptions<ACBSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<SaleDetail> SaleDetail { get; set; }
        public virtual DbSet<SaleHeader> SaleHeader { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("customer_id");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasColumnName("address")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");

                entity.Property(e => e.CustomerName)
                    .IsRequired()
                    .HasColumnName("customer_name")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");

                entity.HasIndex(e => e.ProductId)
                    .HasName("product_id");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inventory)
                    .HasColumnName("inventory")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasColumnName("model")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.OrderPrice)
                    .HasColumnName("order_price")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnName("product_name")
                    .HasColumnType("text")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");

                entity.Property(e => e.SalePrice)
                    .HasColumnName("sale_price")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Warranty)
                    .HasColumnName("warranty")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SaleDetail>(entity =>
            {
                entity.ToTable("sale_detail");

                entity.HasIndex(e => e.ProductId)
                    .HasName("product_id");

                entity.HasIndex(e => e.SoId)
                    .HasName("sale_detail_ibfk_1");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SoId)
                    .HasColumnName("so_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TotalAmount)
                    .HasColumnName("total_amount")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WarrantyEnd)
                    .HasColumnName("warranty_end")
                    .HasColumnType("date");

                entity.Property(e => e.WarrantyStart)
                    .HasColumnName("warranty_start")
                    .HasColumnType("date");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.SaleDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sale_detail_ibfk_2");

                entity.HasOne(d => d.So)
                    .WithMany(p => p.SaleDetails)
                    .HasForeignKey(d => d.SoId)
                    .HasConstraintName("sale_detail_ibfk_1");
            });

            modelBuilder.Entity<SaleHeader>(entity =>
            {
                entity.HasKey(e => e.SoId)
                    .HasName("PRIMARY");

                entity.ToTable("sale_header");

                entity.HasIndex(e => e.CustomerId)
                    .HasName("customer_id");

                entity.Property(e => e.SoId)
                    .HasColumnName("so_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_vietnamese_ci");

                entity.Property(e => e.CustomerId)
                    .HasColumnName("customer_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.IsAtive)
                    .IsRequired()
                    .HasColumnName("isAtive")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("modified_date")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.SubTotal)
                    .HasColumnName("sub_total")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Tax).HasColumnName("tax");

                entity.Property(e => e.TotalLine)
                    .HasColumnName("total_line")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.SaleHeader)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("sale_header_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

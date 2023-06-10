using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartStore.ProductsAPI.Entities;

namespace SmartStore.ProductsAPI.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.Name).IsRequired().HasMaxLength(100);
            builder.Property(P => P.Description).IsRequired();
            builder.Property(P => P.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(P => P.PictureUrl);
            builder.Property(P => P.Quantity).IsRequired();
            
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingCart.Api.Shared.Domain.Entities;

namespace ShoppingCart.Api.Shared.Persistence.Configurations;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("shopping_carts");
        builder.HasKey(x => x.Id);

        builder.HasMany(c => c.Items)
        .WithOne(c => c.Cart)
        .HasForeignKey(c => c.CartId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
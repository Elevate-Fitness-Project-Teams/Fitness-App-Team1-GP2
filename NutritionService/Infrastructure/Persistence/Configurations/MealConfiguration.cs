using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;
using System.Text.Json;

namespace NutritionService.Infrastructure.Persistence.Configurations;

public sealed class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);

        // List<string> columns are persisted as JSON to keep the schema simple (Postgres/SqlServer both support a json/nvarchar column).
        var jsonListConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        builder.Property(m => m.Ingredients).HasConversion(jsonListConverter);
        builder.Property(m => m.Variations).HasConversion(jsonListConverter);
        builder.Property(m => m.Allergens).HasConversion(jsonListConverter);
        builder.Property(m => m.Tags).HasConversion(jsonListConverter);

        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.Calories);
    }
}

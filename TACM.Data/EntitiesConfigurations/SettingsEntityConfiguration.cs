using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TACM.Entities;

namespace TACM.Data.EntitiesConfigurations;

public class SettingsEntityConfiguration : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.ToTable(nameof(Settings).ToLower());
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.GoProbability).HasColumnName("go_probability");
        builder.Property(x => x.RND).HasColumnName("rnd");
        builder.Property(x => x.NoProbability).HasColumnName("non_probability");
        builder.Property(x => x.IsDeleted).HasColumnName("is_delete").HasDefaultValue(false);
        builder.Property(x => x.Trials).HasColumnName("trials");
        builder.Property(x => x.Target).HasColumnName("target").HasMaxLength(1);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
        builder.Property(x => x.FontSize).HasColumnName("font_size");
        builder.Property(x => x.T1).HasColumnName("t1");
        builder.Property(x => x.T2).HasColumnName("t2");
        builder.Property(x => x.T3).HasColumnName("t3");

        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
    }
}

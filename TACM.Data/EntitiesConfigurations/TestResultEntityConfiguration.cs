using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TACM.Entities;

namespace TACM.Data.EntitiesConfigurations
{
    public class TestResultEntityConfiguration : IEntityTypeConfiguration<TestResult>
    {
        public void Configure(EntityTypeBuilder<TestResult> builder)
        {
            builder.ToTable(nameof(TestResult).ToLower());

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.SessionId).IsRequired(true).HasColumnName("session_id");
            builder.HasOne(x => x.Session).WithMany().HasForeignKey(x => x.SessionId);
            builder.Navigation(x => x.Session).EnableLazyLoading(false);
            builder.Navigation(x => x.Items).EnableLazyLoading(false);

            builder.Property(x => x.TestType).HasMaxLength(50).IsRequired(true).HasColumnName("test_type");
            builder.Property(x => x.ItemsCount).IsRequired(true).HasColumnName("items_count").HasDefaultValue(0);
            builder.Property(x => x.Preview).IsRequired(true).HasDefaultValue(false).HasColumnName("preview");

            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

            builder.Property(x => x.End).HasColumnName("end").IsRequired(false);
        }
    }
}

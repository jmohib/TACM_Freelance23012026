using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TACM.Entities;

namespace TACM.Data.EntitiesConfigurations
{
    public class TestResultItemEntityConfiguration : IEntityTypeConfiguration<TestResultItem>
    {
        public void Configure(EntityTypeBuilder<TestResultItem> builder)
        {
            builder.ToTable(nameof(TestResultItem).ToLower());

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(x => x.TestResultId).IsRequired(true).HasColumnName("test_result_id");
            builder.HasOne(x => x.TestResult).WithMany(x => x.Items).HasForeignKey(x => x.TestResultId);

            builder.Property(x => x.SessionId).IsRequired(true).HasColumnName("session_id");
            builder.HasOne(x => x.Session).WithMany().HasForeignKey(x => x.SessionId);

            builder.Property(x => x.Item1).HasColumnName("item1").HasMaxLength(80).IsRequired(true);
            builder.Property(x => x.Item2).HasColumnName("item2").HasMaxLength(80).IsRequired(false);
            builder.Property(x => x.Answer).HasColumnName("answer").IsRequired(true);
            builder.Property(x => x.IsCorrect).HasColumnName("is_correct").IsRequired(true).HasDefaultValue(false);

            builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

            builder.Property(x => x.Start).HasColumnName("start").IsRequired(true);
            builder.Property(x => x.End).HasColumnName("end").IsRequired(true);
        }
    }
}

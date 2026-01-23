using Microsoft.EntityFrameworkCore;
using TACM.Entities;

namespace TACM.Data.DbContextEntitiesExtensions
{
    public static class TestResultsDbContextExtensions
    {
        public static async Task<TestResult> SaveTestResultAsync(this TacmDbContext context, TestResult entity, CancellationToken cancellationToken = default)
        {
            TestResult? saved = null;

            if(entity.Id == 0)
            {
               // entity.Id = Guid.CreateVersion7();
                entity.CreatedAt  = DateTime.UtcNow;
                entity.UpdatedAt = null;
                var entry = await context.TestResults.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                saved = entry.Entity;
            }
            else
            {
                var trackedEntity =
                    context
                        .ChangeTracker
                        .Entries<TestResult>()?
                        .FirstOrDefault(_ => _.Entity?.Id == entity.Id);

                entity.UpdatedAt = DateTime.UtcNow;

                if(trackedEntity is null)
                {
                    context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    saved = entity;
                }
                else
                {
                    trackedEntity.CurrentValues.SetValues(entity);
                    saved = trackedEntity.Entity;
                }
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return saved;
        }

        public static async Task<TestResultItem> SaveTestResultItemAsync(this TacmDbContext context, TestResultItem entity, CancellationToken cancellationToken = default)
        {
            TestResultItem? saved = null;

            if (entity.Id == 0)
            {
               // entity.Id = Guid.CreateVersion7();
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = null;
                var entry = await context.TestResultItems.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                saved = entry.Entity;
            }
            else
            {
                var trackedEntity =
                    context
                        .ChangeTracker
                        .Entries<TestResultItem>()?
                        .FirstOrDefault(_ => _.Entity?.Id == entity.Id);

                entity.UpdatedAt = DateTime.UtcNow;

                if (trackedEntity is null)
                {
                    context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    saved = entity;
                }
                else
                {
                    trackedEntity.CurrentValues.SetValues(entity);
                    saved = trackedEntity.Entity;
                }
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return saved;
        }

        public static async Task<int> GetCorrectAnswerCountAsync(this TacmDbContext context, int testResultId, CancellationToken cancellationToken = default)
        {
            return await context.TestResultItems
                .Where(item => item.TestResultId == testResultId && item.IsCorrect)
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        public static async Task UpdateTestResultEndDateAsync(this TacmDbContext context, int testResultId, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var entity = await 
                context
                    .TestResults
                    .FindAsync([testResultId], cancellationToken)
                    .ConfigureAwait(false);

            if (entity is null)
                return;

            entity.End = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}

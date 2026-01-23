using Microsoft.EntityFrameworkCore;
using TACM.Entities;

namespace TACM.Data.DbContextEntitiesExtensions
{
    public static class SettingsDbContextExtensions
    {
        public static Task<Settings?> GetCurrentSettingsAsync(this TacmDbContext context)
        {
            return context
                .Settings
                .Where(_ => !_.IsDeleted)
                .OrderByDescending(_ => _.CreatedAt)                
                .FirstOrDefaultAsync();
        }

        public static async Task<Settings> SaveSettingsAsync(this TacmDbContext context, Settings settings, CancellationToken cancellationToken = default)
        {
            Settings? saved = null;

            if (settings.Id == 0)
            {
                settings.CreatedAt = DateTime.UtcNow;
                settings.UpdatedAt = null;
                var entry = await context.Settings.AddAsync(settings, cancellationToken).ConfigureAwait(false);
                saved = entry.Entity;
            }
            else
            {
                var trackedEntity =
                    context
                       .ChangeTracker
                       .Entries<Settings>()?
                       .FirstOrDefault(_ => _.Entity?.Id == settings.Id);

                settings.UpdatedAt = DateTime.UtcNow;

                if (trackedEntity is null)
                {
                    context.Entry(settings).State = EntityState.Modified;
                    saved = settings;
                }
                else
                {
                    trackedEntity.CurrentValues.SetValues(settings);
                    saved = trackedEntity.Entity;
                }
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return saved;
        }
    }
}

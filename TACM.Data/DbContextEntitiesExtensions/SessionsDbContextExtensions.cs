using Microsoft.EntityFrameworkCore;
using TACM.Data.Queries;
using TACM.Entities;

namespace TACM.Data.DbContextEntitiesExtensions;

public static class SessionsDbContextExtensions
{
    public static async Task<Session?> SaveSessionAsync(
        this TacmDbContext context, 
        Session? session, 
        CancellationToken cancellationToken = default
    )
    {
        if (session is null)
            return session;

        Session? saved;

        if (session.Id == 0)
        {
           // session.Id = Guid.CreateVersion7();
            session.CreatedAt = DateTime.UtcNow;
            session.UpdatedAt = null;

            var entry = await context.Sessions.AddAsync(session, cancellationToken).ConfigureAwait(false);
            saved = entry.Entity;
        }
        else
        {   
            session.UpdatedAt = DateTime.UtcNow;
            context.Entry(session).State = EntityState.Modified;
            saved = session;
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return saved;
    }

    public static Task<Session?> GetSessionByIdAsync(this TacmDbContext context, int id) =>
        context
            .Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(_ => _.Id == id);

    public static async IAsyncEnumerable<Session> GetSessionsAsync(this TacmDbContext context, GetSessionsQuery? query = default)
    {
        if (query is null)
        {
            await foreach (var item in context.Sessions.AsAsyncEnumerable())
                yield return item;
        }
        else
        {
            await foreach (var item in context
                .Sessions
                .Where(
                    _ => (query.Age == 0 || _.Age == query.Age) &&
                         (query.Sex == null || _.Sex == query.Sex) &&
                         (query.SessionId == null || _.Id == query.SessionId) &&
                         (query.SubjectID == null || _.SubjectID == query.SubjectID) &&
                         (query.SessionDate == null || _.CreatedAt.Date == query.SessionDate.Value.Date)
                )
                .AsAsyncEnumerable()
            )
            {
                yield return item;
            }
        }
    }
}

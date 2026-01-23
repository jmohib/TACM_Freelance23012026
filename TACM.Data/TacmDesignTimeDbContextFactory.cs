using Microsoft.EntityFrameworkCore.Design;

namespace TACM.Data;

public class TacmDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TacmDbContext>
{
    public TacmDbContext CreateDbContext(string[] args)
    {
        return TacmDbContextFactory.CreateDbContext();
    }
}

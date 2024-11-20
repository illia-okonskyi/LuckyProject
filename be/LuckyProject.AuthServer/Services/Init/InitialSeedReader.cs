using LuckyProject.AuthServer.DbLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace LuckyProject.AuthServer.Services.Init
{
    public class InitialSeedReader : IInitialSeedReader
    {
        #region Internals & ctor
        private readonly AuthServerDbContext dbContext;

        public InitialSeedReader(
            IOptions<InitialSeedOptions> options,
            AuthServerDbContext dbContext)
        {
            Options = options.Value;
            this.dbContext = dbContext;
        }
        #endregion

        #region Public interface
        public InitialSeedOptions Options { get; }

        public async Task<DateTime?> GetSeedTimeUtcAsync()
        {
            var seedTimeUtc = await dbContext.InitialSeed
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .Select(s => s.SeedTimeUtc)
                .FirstOrDefaultAsync();
            return seedTimeUtc != default ? seedTimeUtc : null;
        }
        #endregion
    }
}

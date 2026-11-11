using LevelUp.DAL.Common;
using LevelUp.DAL.Contexts;
using LevelUp.DAL.Models;
using LevelUp.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LevelUp.DAL.DataSeed;

public static class ModeRateDataSeeding
{
    private static readonly Dictionary<GameMode, decimal> DefaultRates = new()
    {
        [GameMode.Single] = 100,
        [GameMode.Multi] = 150
    };

    public static async Task SeedAsync(LevelUpDbContext dbContext, ILogger logger)
    {
        try
        {
            foreach (var (mode, hourlyRate) in DefaultRates)
            {
                if (await dbContext.ModeRates.AnyAsync(x => x.Mode == mode))
                    continue;

                dbContext.ModeRates.Add(new ModeRate
                {
                    Mode = mode,
                    HourlyRate = hourlyRate,
                    EffectiveFrom = AppClock.Now
                });

                logger.LogInformation("Mode rate seeded: {Mode} = {HourlyRate} per hour", mode, hourlyRate);
            }

            if (dbContext.ChangeTracker.HasChanges())
                await dbContext.SaveChangesAsync();
            else
                logger.LogInformation("Mode rates seeded already");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Mode rate data seed failed");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.Tests.Helpers;

public static class TestDbFactory
{
    public static RaceDayDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<RaceDayDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new RaceDayDbContext(options);
        context.Database.EnsureCreated(); // runs seeding
        return context;
    }
}
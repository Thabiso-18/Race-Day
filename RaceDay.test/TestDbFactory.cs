using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.test
{
    public static class TestDbFactory
    {
        public static RaceDayDbContext Create(string dbName)
        {
            // 1. Create the options builder
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // 2. Return a new DbContext using those options
            return new RaceDayDbContext(options);
        }
    }
}
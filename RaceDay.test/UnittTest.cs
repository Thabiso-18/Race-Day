using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using Xunit;

namespace RaceDay.test
{
    public class SmokeTests
    {
        [Fact]
        public void DbContext_CanBeCreated()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase
                .Options;

            using var db = new RaceDayDbContext(options);
            Assert.NotNull(db);
        }
    }
}

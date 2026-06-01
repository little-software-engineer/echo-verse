using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWeekndPort.Tests
{
    using TheWeekndPort.Models;
    using Xunit;

    public class ArtistTests
    {
        [Fact]
        public void Artist_Should_Store_Name()
        {
            var artist = new Artist
            {
                Name = "The Weeknd"
            };

            Assert.Equal("The Weeknd", artist.Name);
        }

        [Fact]
        public void Artist_Should_Store_Mood()
        {
            var artist = new Artist
            {
                Mood = "Dark R&B"
            };

            Assert.Equal("Dark R&B", artist.Mood);
        }

        [Fact]
        public void Artist_Should_Have_CreatedAt()
        {
            var artist = new Artist();

            Assert.NotEqual(default(DateTime), artist.CreatedAt);
        }
    }
}

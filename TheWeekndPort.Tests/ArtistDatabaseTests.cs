using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheWeekndPort.Data;
using TheWeekndPort.Models;
using Xunit;

namespace TheWeekndPort.Tests
{
    public class ArtistDatabaseTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private Artist CreateArtist(string name = "The Weeknd")
        {
            return new Artist
            {
                Name = name,
                Bio = "Test bio",
                Era = "dark",
                ImageUrl = "https://test.com/artist.jpg",
                Mood = "dark"
            };
        }

        private Album CreateAlbum(string title)
        {
            return new Album
            {
                Title = title,
                Description = "Test album description",
                Era = "dark",
                ImageUrl = "https://test.com/album.jpg",
                SpotifyEmbedUrl = "https://open.spotify.com/test"
            };
        }

        [Fact]
        public void Artist_With_Albums_Should_Save_And_Retrieve_Correctly()
        {
            // Arrange
            using var context = CreateDbContext();

            var artist = CreateArtist("The Weeknd");

            artist.Albums = new List<Album>
            {
                CreateAlbum("After Hours"),
                CreateAlbum("Starboy")
            };

            context.Artists.Add(artist);
            context.SaveChanges();

            // Act
            var fromDb = context.Artists
                .Include(a => a.Albums)
                .FirstOrDefault(a => a.Name == "The Weeknd");

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal(2, fromDb.Albums.Count);

            Assert.Contains(fromDb.Albums, a => a.Title == "After Hours");
            Assert.Contains(fromDb.Albums, a => a.Title == "Starboy");
        }
    }
}
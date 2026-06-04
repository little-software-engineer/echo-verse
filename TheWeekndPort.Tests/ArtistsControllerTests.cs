using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWeekndPort.Data;
using TheWeekndPort.Models;
using Xunit;

namespace TheWeekndPort.Tests
{
    public class ArtistsControllerTests
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
                ImageUrl = "https://test.com/image.jpg",
                Mood = "dark"
            };
        }

        [Fact]
        public void Index_Returns_ViewResult()
        {
            using var context = CreateDbContext();

            var controller = new ArtistsController(context);

            var result = controller.Index(null, null);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_Returns_All_Artists_When_No_Search_Is_Provided()
        {
            using var context = CreateDbContext();

            context.Artists.Add(CreateArtist("The Weeknd"));
            context.Artists.Add(CreateArtist("Daft Punk"));

            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.Index(null, null) as ViewResult;

            Assert.NotNull(result);

            var model = Assert.IsType<List<Artist>>(result!.Model);

            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Search_Returns_Correct_Artist()
        {
            using var context = CreateDbContext();

            context.Artists.Add(CreateArtist("The Weeknd"));
            context.Artists.Add(CreateArtist("Daft Punk"));

            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.Index("Weeknd", null) as ViewResult;

            Assert.NotNull(result);

            var model = Assert.IsType<List<Artist>>(result!.Model);

            Assert.Single(model);
            Assert.Equal("The Weeknd", model.First().Name);
        }

        [Fact]
        public void Search_With_No_Matches_Returns_Empty_List()
        {
            using var context = CreateDbContext();

            context.Artists.Add(CreateArtist("The Weeknd"));

            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.Index("Metallica", null) as ViewResult;

            Assert.NotNull(result);

            var model = Assert.IsType<List<Artist>>(result!.Model);

            Assert.Empty(model);
        }

        [Fact]
        public void SearchSuggestions_Returns_JsonResult()
        {
            using var context = CreateDbContext();

            context.Artists.Add(CreateArtist("The Weeknd"));
            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.SearchSuggestions("Weeknd");

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void Details_Returns_ViewResult_When_Artist_Exists()
        {
            using var context = CreateDbContext();

            var artist = CreateArtist("The Weeknd");

            context.Artists.Add(artist);
            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.Details(artist.Id);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Timeline_Returns_ViewResult_When_Artist_Exists()
        {
            using var context = CreateDbContext();

            var artist = CreateArtist("The Weeknd");

            context.Artists.Add(artist);
            context.SaveChanges();

            var controller = new ArtistsController(context);

            var result = controller.Timeline(artist.Id);

            Assert.IsType<ViewResult>(result);
        }
    }
}
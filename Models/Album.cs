namespace TheWeekndPort.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; }
        public string Era { get; set; }

        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public List<Inspiration> Inspirations { get; set; }

        public List<Track> Tracks { get; set; }
    }
}

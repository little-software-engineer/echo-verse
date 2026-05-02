namespace TheWeekndPort.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Mood { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}

namespace TheWeekndPort.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }

        public string ImageUrl { get; set; }

        public string Era { get; set; }   // e.g. dark, neon, classic
        public string Mood { get; set; }  // e.g. emotional, energetic, dark

        public List<Album> Albums { get; set; }
    }
}
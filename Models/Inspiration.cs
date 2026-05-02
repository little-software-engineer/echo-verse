using System.ComponentModel.DataAnnotations.Schema;
using TheWeekndPort.Models;

[Table("Inspirations")]
public class Inspiration
{
    public int Id { get; set; }

    public string Title { get; set; }
    public string Type { get; set; }
    public string ImageUrl { get; set; }

    public int AlbumId { get; set; }
    public Album Album { get; set; }
}

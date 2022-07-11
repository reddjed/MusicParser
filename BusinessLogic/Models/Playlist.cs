namespace BusinessLogic.Models
{
    public class Playlist
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<Song>? Songs { get; set; } = new List<Song>();

    }
}

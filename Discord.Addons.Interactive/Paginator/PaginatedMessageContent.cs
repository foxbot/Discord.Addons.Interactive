namespace Discord.Addons.Interactive
{
    public class PaginatedMessageContent
    {
        public string Title { get; set; }
        public EmbedAuthorBuilder Author { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public Color Color { get; set; } = Color.Default;
    }
}

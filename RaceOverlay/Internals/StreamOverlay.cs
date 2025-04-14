namespace RaceOverlay.Internals;

public abstract class StreamOverlay
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    
    public StreamOverlay(string title, string description, string link)
    {
        Title = title;
        Description = description;
        Link = link;
    }
    
}
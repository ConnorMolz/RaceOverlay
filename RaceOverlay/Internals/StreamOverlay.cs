namespace RaceOverlay.Internals;

/// <summary>
/// StreamOverlay is only for UI Reasons currently used.
/// </summary>
public abstract class StreamOverlay
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    
    /// <summary>
    /// StreamOverlay is only for UI Reasons currently used.
    /// </summary>
    /// <param name="title">Title for UI.</param>
    /// <param name="description">Description for UI.</param>
    /// <param name="link">Link which the User will copy for use the stream overlay in streaming software like OBS</param>
    public StreamOverlay(string title, string description, string link)
    {
        Title = title;
        Description = description;
        Link = link;
    }
    
}
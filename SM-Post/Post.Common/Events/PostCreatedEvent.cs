using CQRS.Core.Events;

namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent
{
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {        
    }

    public required string Author { get; set; }
    public required string Message { get; set; }
    public required DateTime DatePosted { get; set; }
}
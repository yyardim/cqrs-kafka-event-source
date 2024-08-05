using System;
using CQRS.Core.Events;

namespace Post.Common.Events;

public class CommentUpdatedEvent : BaseEvent
{
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
    {
    }

    public required Guid CommentId { get; set; }
    public required string Comment { get; set; }
    public required string Username { get; set; }
    public required DateTime EditDate { get; set; }
}
using CQRS.Core.Messages;

namespace CQRS.Core.Events;

public abstract class BaseEvent(string type) : Message
{
    public int Version { get; set; }
    public string Type { get; set; } = type;
}
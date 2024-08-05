using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher: ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = [];

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Handler for command '{typeof(T).Name}' was already registered.");
        }

        _handlers.Add(typeof(T), command => handler((T)command));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
        {
            await handler(command);
        }
        else
        {
            throw new InvalidOperationException($"Handler for command '{command.GetType().Name}' was not found.");
        }
    }
}
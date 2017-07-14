using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System;

namespace Discord.Addons.Interactive
{
    public interface IReactionCallback
    {
        RunMode RunMode { get; }
        ICriterion<SocketReaction> Criterion { get; }
        TimeSpan? Timeout { get; }
        SocketCommandContext Context { get; }

        Task DisplayAsync();
        Task<bool> HandleCallbackAsync(SocketReaction reaction);
    }
}

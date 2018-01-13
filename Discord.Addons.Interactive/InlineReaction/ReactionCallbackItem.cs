using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class ReactionCallbackItem
    {
        public IEmote Reaction { get; }
        public Func<SocketCommandContext, Task> Callback { get; }

        public ReactionCallbackItem(IEmote reaction, Func<SocketCommandContext, Task> callback)
        {
            Reaction = reaction;
            Callback = callback;
        }
    }
}

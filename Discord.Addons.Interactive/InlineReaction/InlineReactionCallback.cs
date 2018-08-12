using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class InlineReactionCallback : IReactionCallback
    {
        public RunMode RunMode => RunMode.Sync;

        public ICriterion<SocketReaction> Criterion { get; }

        public TimeSpan? Timeout { get; }

        public SocketCommandContext Context { get; }

        public IUserMessage Message { get; private set; }

        private readonly InteractiveService _interactive;
        private readonly ReactionCallbackData _data;

        public InlineReactionCallback(
            InteractiveService interactive,
            SocketCommandContext context,
            ReactionCallbackData data,
            ICriterion<SocketReaction> criterion = null)
        {
            _interactive = interactive;
            Context = context;
            _data = data;
            Criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            Timeout = data.Timeout ?? TimeSpan.FromSeconds(30);
        }

        public async Task DisplayAsync()
        {
            var message = await Context.Channel.SendMessageAsync(_data.Text, embed: _data.Embed).ConfigureAwait(false);
            Message = message;
            _interactive.AddReactionCallback(message, this);

            _ = Task.Run(async () =>
            {
                foreach (var item in _data.Callbacks)
                    await message.AddReactionAsync(item.Reaction);
            });

            if (Timeout.HasValue)
            {
                _ = Task.Delay(Timeout.Value)
                    .ContinueWith(_ => _interactive.RemoveReactionCallback(message));
            }
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var reactionCallbackItem = _data.Callbacks.FirstOrDefault(t => t.Reaction.Equals(reaction.Emote));
            if (reactionCallbackItem == null)
                return false;

            await reactionCallbackItem.Callback(Context);
            return true;
        }
    }
}

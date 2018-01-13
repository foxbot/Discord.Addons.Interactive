using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Addons.Interactive
{
    public class ReactionCallbackData
    {
        private readonly ICollection<ReactionCallbackItem> items;

        public string Text { get; }
        public Embed Embed { get; }
        public TimeSpan? Timeout { get; }
        public IEnumerable<ReactionCallbackItem> Callbacks => items;

        public ReactionCallbackData(string text, Embed embed = null, TimeSpan? timeout = null)
        {
            Text = text;
            Embed = embed;
            Timeout = timeout;
            items = new List<ReactionCallbackItem>();
        }

        public ReactionCallbackData WithCallback(IEmote reaction, Func<SocketCommandContext, Task> callback)
        {
            var item = new ReactionCallbackItem(reaction, callback);
            items.Add(item);
            return this;
        }
    }
}

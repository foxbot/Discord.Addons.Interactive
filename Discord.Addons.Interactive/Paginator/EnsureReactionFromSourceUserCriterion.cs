using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Addons.Interactive
{
    internal class EnsureReactionFromSourceUserCriterion
        : ICriterion<SocketReaction>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketReaction parameter)
        {
            return Task.FromResult(parameter.UserId == sourceContext.User.Id);
        }
    }
}

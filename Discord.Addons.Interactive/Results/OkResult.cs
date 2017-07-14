using Discord.Commands;

namespace Discord.Addons.Interactive
{
    public class OkResult : RuntimeResult
    {
        public OkResult(string reason = null) : base(null, reason) { }
    }
}

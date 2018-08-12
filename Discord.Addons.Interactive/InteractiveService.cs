﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Discord.Addons.Interactive
{
    public class InteractiveService : IDisposable
    {
        public DiscordSocketClient Discord { get; }

        private Dictionary<ulong, IReactionCallback> _callbacks;
        private TimeSpan _defaultTimeout;

        public InteractiveService(DiscordSocketClient discord, TimeSpan? defaultTimeout = null)
        {
            Discord = discord;
            Discord.ReactionAdded += HandleReactionAsync;

            _callbacks = new Dictionary<ulong, IReactionCallback>();
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(15);
        }

        public Task<SocketMessage> NextMessageAsync(SocketCommandContext context, bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null)
        {
            var criterion = new Criteria<SocketMessage>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureSourceUserCriterion());
            if (inSourceChannel)
                criterion.AddCriterion(new EnsureSourceChannelCriterion());
            return NextMessageAsync(context, criterion, timeout);
        }
        public async Task<SocketMessage> NextMessageAsync(SocketCommandContext context, ICriterion<SocketMessage> criterion, TimeSpan? timeout = null)
        {
            timeout = timeout ?? _defaultTimeout;

            var eventTrigger = new TaskCompletionSource<SocketMessage>();

            async Task Handler(SocketMessage message)
            {
                var result = await criterion.JudgeAsync(context, message).ConfigureAwait(false);
                if (result)
                    eventTrigger.SetResult(message);
            }

            context.Client.MessageReceived += Handler;

            var trigger = eventTrigger.Task;
            var delay = Task.Delay(timeout.Value);
            var task = await Task.WhenAny(trigger, delay).ConfigureAwait(false);

            context.Client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger.ConfigureAwait(false);
            else
                return null;
        }

        public async Task<IUserMessage> ReplyAndDeleteAsync(SocketCommandContext context, string content, bool isTTS = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
        {
            timeout = timeout ?? _defaultTimeout;
            var message = await context.Channel.SendMessageAsync(content, isTTS, embed, options).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value)
                .ContinueWith(_ => message.DeleteAsync().ConfigureAwait(false))
                .ConfigureAwait(false);
            return message;
        }

        public async Task<IUserMessage> SendPaginatedMessageAsync(SocketCommandContext context, PaginatedMessage pager, ICriterion<SocketReaction> criterion = null)
        {
            var callback = new PaginatedMessageCallback(this, context, pager, criterion);
            await callback.DisplayAsync().ConfigureAwait(false);
            return callback.Message;
        }

        public async Task<IUserMessage> SendMessageWithReactionCallbacksAsync(SocketCommandContext context, ReactionCallbackData callbacks, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if(fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            var callback = new InlineReactionCallback(this, context, callbacks, criterion);
            await callback.DisplayAsync().ConfigureAwait(false);
            return callback.Message;
        }

        public void AddReactionCallback(IMessage message, IReactionCallback callback)
            => _callbacks[message.Id] = callback;
        public void RemoveReactionCallback(IMessage message)
            => RemoveReactionCallback(message.Id);
        public void RemoveReactionCallback(ulong id)
            => _callbacks.Remove(id);
        public void ClearReactionCallbacks()
            => _callbacks.Clear();

        private async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == Discord.CurrentUser.Id) return;
            if (!(_callbacks.TryGetValue(message.Id, out var callback))) return;
            if (!(await callback.Criterion.JudgeAsync(callback.Context, reaction).ConfigureAwait(false)))
                return;
            switch (callback.RunMode)
            {
                case RunMode.Async:
                    _ = Task.Run(async () =>
                    {
                        if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                            RemoveReactionCallback(message.Id);
                    });
                    break;
                default:
                    if (await callback.HandleCallbackAsync(reaction).ConfigureAwait(false))
                        RemoveReactionCallback(message.Id);
                    break;
            }
        }

        public void Dispose()
        {
            Discord.ReactionAdded -= HandleReactionAsync;
        }
    }
}

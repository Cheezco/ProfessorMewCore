using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Poll
{
    public class Poll : IDisposable
    {
        public string Title { get; private set; }
        public List<string> Choices { get; private set; }
        public System.Timers.Timer Timer { get; private set; }
        public double Duration { get; private set; }
        public IUserMessage Message { get; private set; }
        public event EventHandler WorkCompleted;

        private DateTime _timerStart;
        private bool _disposed;
        private bool _locked;
        public static readonly List<PollEmote> Emotes = new List<PollEmote>()
        {
            new PollEmote(":one:", "1️⃣"),
            new PollEmote(":two:", "2️⃣"),
            new PollEmote(":three:", "3️⃣"),
            new PollEmote(":four:", "4️⃣"),
            new PollEmote(":five:", "5️⃣"),
            new PollEmote(":six:", "6️⃣"),
            new PollEmote(":seven:", "7️⃣"),
            new PollEmote(":eight:", "8️⃣"),
            new PollEmote(":nine:", "9️⃣")
        };
        public Poll(double duration, string title)
        {
            Duration = TimeSpan.FromMinutes(duration).TotalMilliseconds;
            Title = title;
            Choices = new List<string>();
            _locked = false;
            _disposed = false;
        }

        public void AddChoice(string choice)
        {
            if (_locked) return;

            Choices.Add(choice);
        }
        public async Task StartAsync(ITextChannel textChannel)
        {
            if (_locked) return;

            _ = textChannel ?? throw new NullReferenceException("StartAsync");

            Message = await textChannel.SendMessageAsync(embed: EmbedUtils.CreatePollEmbed(this));

            await Message.AddReactionsAsync(GetEmojis());
            SetupTimer();
            _timerStart = DateTime.UtcNow;
            _locked = true;
            Timer.Start();

        }
        public void Stop()
        {
            if (Timer is null) return;

            Timer.Stop();
            OnWorkCompleted(EventArgs.Empty);
        }

        public async Task EndAsync()
        {
            if (Timer is null) return;

            Timer.Stop();
            await Timer_Elapsed();
        }
        public void ExtendDuration(int duration)
        {
            if (Timer is null) return;

            var timeLeft = _timerStart.AddMilliseconds(Duration) - DateTime.UtcNow;
            var newTime = timeLeft.Add(new TimeSpan(0, duration, 0));
            Duration = newTime.TotalMilliseconds;
            Timer.Stop();
            SetupTimer();
            Timer.Start();
        }

        private void SetupTimer()
        {
            if (_locked) return;

            if(Timer is null)
            {
                Timer = new System.Timers.Timer();
                Timer.Elapsed += async delegate { await Timer_Elapsed(); };
                Timer.AutoReset = false;
            }
            Timer.Interval = Duration;
            
        }
        private Emoji[] GetEmojis()
        {
            int count = Choices.Count > 9 ? 9 : Choices.Count;
            return Emotes.Where(x => Emotes.IndexOf(x) < count)
                .Select(x => x.Emoji)
                .ToArray();
        }

        private async Task Timer_Elapsed()
        {
            if(Message is RestUserMessage message)
            {
                await message.UpdateAsync();

                var votes = new Dictionary<IEmote, int>();

                foreach(var reaction in message.Reactions)
                {
                    var reactionUsers = await message.GetReactionUsersAsync(reaction.Key, 500).FlattenAsync();

                    if (reactionUsers is null || !reactionUsers.Any(x => x.IsBot)) continue;

                    votes.Add(reaction.Key, reaction.Value.ReactionCount - 1);
                }

                await Message.Channel.SendMessageAsync(embed: EmbedUtils.CreatePollResultEmbed(this, votes));
            }

            _locked = false;
            OnWorkCompleted(EventArgs.Empty);
        }

        private void OnWorkCompleted(EventArgs e)
        {
            EventHandler handler = WorkCompleted;
            handler?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (_disposed) return;

            Timer.Dispose();
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }

    public class PollEmote
    {
        public string Discord { get; private set; }
        public Emoji Emoji { get; private set; }

        public PollEmote(string discord, string unicode)
        {
            Discord = discord;
            Emoji = new Emoji(unicode);
        }
    }
}

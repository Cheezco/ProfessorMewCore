using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Poll
{
    public sealed class PollManager
    {
        private static readonly Lazy<PollManager> lazy = new Lazy<PollManager>(() => new PollManager());
        private readonly List<Poll> _polls;

        public static PollManager Instance { get { return lazy.Value; } }

        private PollManager()
        {
            _polls = new List<Poll>();
        }

        public void Add(Poll poll)
        {
            _polls.Add(poll);
            poll.WorkCompleted += Poll_WorkCompleted;
        }

        public Poll Get(string title)
        {
            return _polls.FirstOrDefault(x => x.Title.ToUpperInvariant().Contains(title.ToUpperInvariant()));
        }

        public bool Contains(string title)
        {
            if(Get(title) is null)
            {
                return false;
            }

            return true;
        }

        public void Stop(string title)
        {
            var poll = Get(title);

            if (poll is null) return;

            poll.Stop();
        }

        public void Extend(string title, int duration)
        {
            var poll = Get(title);

            if (poll is null) return;

            poll.ExtendDuration(duration);
        }

        public async Task EndAsync(string title)
        {
            var poll = Get(title);

            await poll.EndAsync();
        }

        private void Poll_WorkCompleted(object sender, EventArgs e)
        {
            if(sender is Poll poll)
            {
                poll.Dispose();
                _polls.Remove(poll);
            }
        }
    }
}

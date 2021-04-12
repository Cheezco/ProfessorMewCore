using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Services
{
    public class ServerProtectionService
    {
        private List<ProtectionData> _protectionDatas = new List<ProtectionData>();
        public bool CheckForSpam(SocketUser user, string message)
        {
            if (message.Length < 2 || user is not SocketGuildUser guildUser) return false;

            if(!_protectionDatas.Any(x => x.UserID == user.Id))
            {
                _protectionDatas.Add(new ProtectionData(user.Id));
                return false;
            }

            var protectionData = _protectionDatas.First(x => x.UserID == user.Id);
            int similarMessages = 0;
            var messages = new List<string>();
            foreach(var receivedMessage in protectionData.ReceivedMessages.Where(x => x.Key == guildUser.Guild.Id))
            {
                if(message.Length - GetLevenshteinDistance(message, receivedMessage.Value) / message.Length >= 0.9)
                {
                    similarMessages++;
                    messages.Add(receivedMessage.Value);
                }
            }


            int messagesWithinRange = 0;
            int pos = 0;
            var receivedMessages = protectionData.ReceivedMessages.Where(x => x.Key == guildUser.Guild.Id).Select(x => x.Value).ToList();
            foreach(var foundMessage in messages)
            {
                
                if(receivedMessages.IndexOf(foundMessage) - pos < 3)
                {
                    messagesWithinRange++;
                }
            }

            if(messagesWithinRange >= 2)
            {
                protectionData.Marks[guildUser.Guild.Id]++;
            }

            protectionData.ReceivedMessages.Add(guildUser.Guild.Id, message);

            if(protectionData.Marks[guildUser.Guild.Id] >= 3)
            {
                protectionData.Marks[guildUser.Guild.Id] = 0;
                return true;
            }

            return false;
        }
        public async Task MuteAsync(SocketGuildUser user, Discord.IRole role)
        {
            var protectionData = _protectionDatas.FirstOrDefault(x => x.GuildID == user.Guild.Id && x.UserID == user.Id);
            await protectionData.StartTimerAsync(user, role);
        }
        private static int GetLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }

    public class ProtectionData
    {
        public ulong GuildID { get; set; }
        public ulong UserID { get; set; }
        //public List<string> ReceivedMessages { get; set; }
        public Dictionary<ulong, string> ReceivedMessages { get; set; }
        public Dictionary<ulong, int> Marks { get; set; }
        private Dictionary<ulong, System.Timers.Timer> Timers;

        private Dictionary<ulong, SocketGuildUser> Users;
        private Dictionary<ulong, Discord.IRole> Roles;
        public ProtectionData(ulong userID)
        {
            UserID = userID;
            //ReceivedMessages = new List<string>();
            ReceivedMessages = new Dictionary<ulong, string>();
            //Marks = 0;
            Marks = new Dictionary<ulong, int>();
            Users = new Dictionary<ulong, SocketGuildUser>();
            Roles = new Dictionary<ulong, Discord.IRole>();
        }

        public async Task StartTimerAsync(SocketGuildUser user, Discord.IRole role)
        {
            if(!Users.ContainsKey(user.Guild.Id))
            {
                Users.Add(user.Guild.Id, user);
            }
            if(!Roles.ContainsKey(user.Guild.Id))
            {
                Roles.Add(user.Guild.Id, role);
            }
            await user.RemoveRoleAsync(role);
            SetupTimer(user.Guild.Id);
            Timers[user.Guild.Id].Start();
        }

        private void SetupTimer(ulong guildID)
        {
            if (!Timers.ContainsKey(guildID))
            {
                var timer = new System.Timers.Timer(TimeSpan.FromMinutes(5).Milliseconds);
                timer.AutoReset = false;
                timer.Elapsed += async delegate { await Timer_Elapsed(guildID); };
                Timers.Add(guildID, timer);
            }
        }

        private async Task Timer_Elapsed(ulong guildID)
        {
            await Users[guildID].AddRoleAsync(Roles[guildID]);
        }
    }
}

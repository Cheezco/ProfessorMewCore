using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
using System.Timers;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using ProfessorMewData.Enums.Guild;
using ProfessorMewData.Contexts;
using ProfessorMewData.Extensions.Guild;
using ProfessorMewData.Interfaces.Guild;
using ProfessorMewData.Models.Guild;
using Microsoft.EntityFrameworkCore;

namespace ProfessorMewCore.Services
{
    public class GuildService
    {
        private readonly DiscordSocketClient _discord;

        private List<SavedGuild> _guilds;
        private Timer _timer;

        public GuildService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
        }

        public async Task InitializeAsync()
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                _guilds = await context.Guilds
                    .Include(x => x.Channels)
                    .Include(x => x.Users)
                    .ToListAsync();
            }
            SetupTimer();
            if (!_timer.Enabled) 
            {
                _timer.Start();
            }
        }

        public async Task UpdateAsync()
        {
            _guilds.Clear();
            await InitializeAsync();
        }

        public bool CanExecuteCommand(ulong guildID, ulong channelID)
        {
            var guild = _guilds.GetGuild(guildID);

            if (guild is null) return true;

            var channel = guild.GetChannel(channelID);

            if (channel is null) return false;

            return channel.ExecuteCommands;
        }

        private void SetupTimer()
        {
            if (_timer is not null) return;

            _timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(10).TotalMilliseconds,
                AutoReset = true
            };
            _timer.Elapsed += async delegate { await ResetMonthlyPointsAsync(); };
        }

        private async Task ResetMonthlyPointsAsync()
        {
#if DEBUG
            return;
#endif
#pragma warning disable CS0162 // Unreachable code detected
            if (DateTime.UtcNow.AddDays(1).Month == DateTime.UtcNow.Month) return;
#pragma warning restore CS0162 // Unreachable code detected
            if (System.IO.File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Data/Txt/MonthlyReset.txt"))
            {
                string text = await System.IO.File.ReadAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}Data/Txt/MonthlyReset.txt");
                if (text == DateTime.UtcNow.Month.ToString()) return;
            }

            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                /*var guilds = await context.Guilds
                    .Include(x => x.Users)
                    .Include(x => x.Channels)
                    .ToListAsync();*/
                var guilds = await context.Guilds
                    .Include(x => x.Users)
                    .Include(x => x.Channels)
                    .ToListAsync();

                foreach(var guild in guilds)
                {
                    if (guild.Users is null || guild.Users.Count == 0) continue;

                    var users = guild.GetUsers(PointDisplay.ShowAll).ToList();
                    users.RemoveNonExistingUsers(_discord.GetGuild(guild.DiscordID), _discord);
                    if (users.Count == 0) continue;

                    users.Sort(
                        delegate (IUser u1, IUser u2)
                        {
                            return u2.MonthPoints.CompareTo(u1.MonthPoints);
                        });
                    var discordGuild = Misc.DiscordUtils.GetGuild(_discord.Guilds, guild.DiscordID);
                    var channel = guild.GetChannel("AdminChannel");
                    var textChannel = (discordGuild as SocketGuild).GetTextChannel(channel.DiscordID);

                    if (textChannel is null) continue;

                    await textChannel.SendMessageAsync($"<@{users[0].DiscordID}> earned most points this month");
                    
                    foreach(var user in guild.Users)
                    {
                        user.MonthPoints = 0;
                    }
                }

                await context.SaveChangesAsync();
            }
            await System.IO.File.WriteAllTextAsync($"{AppDomain.CurrentDomain.BaseDirectory}Data/Txt/MonthlyReset.txt", DateTime.UtcNow.Month.ToString());
        }
    }
}

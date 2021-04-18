using Discord.Addons.Interactive;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ProfessorMewCore.Preconditions;
using ProfessorMewData.Extensions.Guild;
using ProfessorMewData.Contexts;
using ProfessorMewData.Enums.Guild;
using Microsoft.EntityFrameworkCore;

namespace ProfessorMewCore.Modules.Lottery
{
    [Group("Lottery")]
    public class LotteryModule : InteractiveBase
    {
        [Command("Winner")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task GetWinnerAsync([Remainder]string prizes)
        {
            if(string.IsNullOrEmpty(prizes))
            {
                await ReplyAsync("Error");
                return;
            }

            var splitPrizes = new List<string>(prizes.Split(','));

            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Channels)
                    .Include(x => x.Links)
                    .Include(x => x.LotteryUsers)
                    .ThenInclude(x => x.User)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());

                if(guild is null)
                {
                    await ReplyAsync("Guild not found.");
                    return;
                }

                var users = guild.LotteryUsers.ToList();
                var savedChannel = guild.GetChannel("Lottery");
                var channel = Context.Guild.GetTextChannel(savedChannel.DiscordID);
                var savedLink = guild.GetLink("LotteryWinner");

                var lotteryGame = new LotteryGame(users, splitPrizes);
                var winners = lotteryGame.GetWinners(splitPrizes.Count, true);
                
                winners.ForEach(x => x.Name = x.User.GetName(Context));
                await channel.SendMessageAsync(embed: EmbedUtils.CreateWinnerEmbed(winners, splitPrizes, savedLink.URL));
                
                guild.LotteryUsers.Clear();
                await context.SaveChangesAsync();
            }
        }

        [Command("Add")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddTicketsAsync(IGuildUser guildUser, int tickets)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var user = await context.LotteryUsers
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild.DBDiscordID == Context.Guild.Id.ToString());

                if(user is null)
                {
                    await ReplyAsync("User not found.");
                    return;
                }

                user.AddTickets(tickets);
                await context.SaveChangesAsync();

                await ReplyAsync($"<@{user.DiscordID}> now has {user.Tickets} tickets");
            }
        }
        
        [Command("Reduce")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task ReduceTicketsAsync(IGuildUser guildUser, int tickets)
        {
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var user = await context.LotteryUsers
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == guildUser.Id.ToString() && x.Guild.DBDiscordID == Context.Guild.Id.ToString());

                if (user is null)
                {
                    await ReplyAsync("User not found.");
                    return;
                }

                user.ReduceTickets(tickets);
                await context.SaveChangesAsync();

                await ReplyAsync($"<@{user.DiscordID}> now has {user.Tickets} tickets");
            }
        }

        [Command("Delete")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task DeleteLotteryAsync()
        {
            using (var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.Messages)
                    .Include(x => x.Channels)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());
                var savedChannel = guild.GetChannel("Lottery");
                var savedMessage = guild.GetMessage("LotteryMessage");

                var channel = Context.Guild.GetTextChannel(savedChannel.DiscordID);
                await Misc.DiscordUtils.DeleteMessageAsync(channel, savedMessage.DiscordID);
                guild.Messages.Remove(savedMessage);


                await context.SaveChangesAsync();
            }

            await ReplyAsync("Lottery deleted");
        }

        [Command("Update")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task UpdateTicketsAsync()
        {
            bool finished = false;
            using(var context = new GuildContext())
            {
                await context.Database.EnsureCreatedAsync();

                var guild = await context.Guilds
                    .Include(x => x.LotteryUsers)
                    .ThenInclude(x => x.User)
                    .Include(x => x.Channels)
                    .Include(x => x.Messages)
                    .SingleOrDefaultAsync(x => x.DBDiscordID == Context.Guild.Id.ToString());
                var users = guild.LotteryUsers.ToList();
                var savedChannel = guild.GetChannel("Lottery");
                var channel = Context.Guild.GetTextChannel(savedChannel.DiscordID);

                var savedMessage = guild.GetMessage("LotteryMessage");

                var message = await channel.GetMessageAsync(savedChannel.DiscordID);
                if (message is SocketUserMessage socketMessage)
                {
                    users.ForEach(x => x.Name = x.User.GetName(Context));
                    await socketMessage.ModifyAsync(x => x.Embed = EmbedUtils.CreateParticipantEmbed(users));
                    finished = true;
                }

                if (finished) return;

                await channel.SendMessageAsync(embed: EmbedUtils.CreateParticipantEmbed(users));
            }
        }
    }
}

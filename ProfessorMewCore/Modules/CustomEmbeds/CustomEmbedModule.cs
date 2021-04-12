using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using ProfessorMewCore.Preconditions;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System;
using ProfessorMewData.Enums.Guild;

namespace ProfessorMewCore.Modules.CustomEmbeds
{
    [Group("Embed")]
    public class CustomEmbedModule : InteractiveBase
    {
        private readonly HttpClient _client;

        public CustomEmbedModule(HttpClient client)
        {
            _client = client;
        }

        [Command("Send")]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task SendEmbedPresetAsync(ITextChannel channel, string presetName)
        {
            if(!EmbedPreset.PresetExists(presetName))
            {
                await ReplyAsync("Preset not found");
                return;
            }

            await channel.SendMessageAsync(embed: EmbedPreset.LoadPreset(presetName).ConstructEmbed());
        }

        [Command("List")]
        [RequirePermission(Privileges.Moderator)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task GetEmbedPresetListAsync()
        {
            var fileNames = EmbedPreset.GetPresetNames();
            string message = "Embed presets:\n```";
            fileNames.ForEach(x => message += x + "\n");
            message += "```";

            await ReplyAsync(message);
        }

        [Command("New")]
        [RequirePermission(Privileges.Admin)]
        [RequireContext(ContextType.Guild)]
        [LimitChannel]
        public async Task AddNewEmbedPresetAsync(string name)
        {
            if(EmbedPreset.PresetExists(name))
            {
                await ReplyAsync("Preset with that name already exists");
                return;
            }
            if(Context.Message.Attachments.Count == 0)
            {
                await ReplyAsync("No attachment found");
                return;
            }
            var attachment = Context.Message.Attachments.FirstOrDefault(x => x.Filename[(x.Filename.LastIndexOf('.') + 1)..] == "txt");
            if(attachment is null)
            {
                await ReplyAsync("No valid attachements found");
                return;
            }
            if(attachment.Size > 500000)
            {
                await ReplyAsync("File exceeds size limit");
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, attachment.Url);
            var response = await _client.SendAsync(request);
            if(!response.IsSuccessStatusCode)
            {
                await ReplyAsync("Failed to get attachment");
                return;
            }
            try
            {
                string content = await response.Content.ReadAsStringAsync();
                int objNameIndex = content.IndexOf("\"embed\": ");
                content = content.Substring(objNameIndex + 9, content.LastIndexOf('}') - objNameIndex - 9);
                EmbedPreset.ParsePreset(content).SavePreset(name);
                await ReplyAsync("Embed preset created");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ReplyAsync("Error");
                return;
            }
        }
    }
}

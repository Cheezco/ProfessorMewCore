using Discord;
using ProfessorMewData.Enums.Raid;
using ProfessorMewData.Interfaces.Raid;
using ProfessorMewData.Extensions.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Raid
{
    public static class EmbedUtils
    {
        private static readonly Dictionary<Role, string> _roleTitles = new Dictionary<Role, string>()
        {
            {Role.Power, "⚔️ Power" },
            {Role.Condi, "🩸 Condi" },
            {Role.Healer, "❤️ Healer" },
            {Role.Boon, "⏫ Boon" },
            {Role.Alac, "🕐 Alac" },
            {Role.Bannerslave, "🏳️ Bannerslave" },
            {Role.Quickness, "⚡ Quickness" },
            {Role.Tank, "🛡️ Tank" },
        };

        private static readonly Dictionary<BenchStatus, string> _benchEmoji = new Dictionary<BenchStatus, string>()
        {
            { BenchStatus.Failed, "❌" },
            { BenchStatus.None, "🔘" },
            { BenchStatus.Passed, "✅" },
            { BenchStatus.Unkown, "❔" }
        };

        public static Embed CreatePlayerNotFoundEmbed()
        {
            return new EmbedBuilder()
                .WithTitle("Player not found.")
                .WithColor(Discord.Color.Red)
                .WithImageUrl("https://2.bp.blogspot.com/-Sup3fJ8XOwo/XYeIaosTIzI/AAAAAAABbAI/5WFwvlFP1q0osmR_QWs9vQ-eZhxodZ98QCLcBGAsYHQ/s1600/Karma-Icon.jpg")
                .Build();
        }

        public static Embed CreateBenchEmbed(ICollection<IRaidBench> raidBenches, int currentPageIndex, int pageCount)
        {
            if (raidBenches is null || raidBenches.Count == 0)
            {
                return new EmbedBuilder()
                    .WithTitle("Error")
                    .Build();
            }
            var embedBuilder = new EmbedBuilder()
                .WithDescription($"Viewing page {currentPageIndex}/{pageCount}")
                .WithCurrentTimestamp();

            var allContent = GetAllContent(raidBenches);
            foreach(var kvp in allContent)
            {
                embedBuilder.AddField($"**{kvp.Key}**", kvp.Value);
            }

            return embedBuilder.Build();
        }

        public static Embed CreateDPSProfileEmbed(ICollection<IRaidRecord> records, int currentPageIndex, int pageCount, string thumbnailLink = "")
        {
            if (records is null)
            {
                return new EmbedBuilder()
                    .WithTitle("Error")
                    .Build();
            }

            var embedBuilder = new EmbedBuilder()
                .WithDescription($"Viewing page {currentPageIndex}/{pageCount}")
                .WithCurrentTimestamp()
                .WithThumbnailUrl(@"https://4.bp.blogspot.com/-jJKf5m3-1bM/XYeIZru1xAI/AAAAAAABa_4/-ghMslJrmTg7dhFveFQQpMDSVU2jdMc9gCLcBGAsYHQ/s1600/Downed-Enemy-Icon.jpg");
            var allContent = GetAllContent(records);
            foreach(var kvp in allContent)
            {
                embedBuilder.AddField($"**{kvp.Key}**", kvp.Value);
            }

            if(embedBuilder.Fields.Count == 0)
            {
                embedBuilder
                    .WithDescription("No characters found.");
            }

            return embedBuilder.Build();
        }

        private static List<KeyValuePair<string, string>> GetAllContent(ICollection<IRaidBench> raidBenches)
        {
            var allContent = new List<KeyValuePair<string, string>>();
            var grouping = raidBenches.GroupBy(x => x.Role);
            foreach(var group in grouping)
            {
                string content = string.Empty;
                foreach(var bench in group)
                {
                    content += $":pushpin:`ID: {bench.ID}`\n:tools:`Role: {bench.Role}`\n:hammer:`Class: {bench.Class}`\n:wrench:`Specialization: {bench.Specialization}`\n";
                    if(bench.DPS > 0)
                    {
                        content += $":bar_chart:`DPS: {bench.GetConvertedDPS()}`\n:page_facing_up:`Scale: {bench.Scale}`\n:bookmark_tabs:`DPS(with scale applied): {bench.GetConvertedDPS(true)}`\n";
                    }
                    if(bench.BoonUptime > 0)
                    {
                        content += $":clock6:`Boon uptime: {bench.BoonUptime}%`\n";
                    }
                    content += "\n";
                    //content += $":pushpin:`ID: {bench.ID}`\n:tools:`Role: {bench.Role}`\n:hammer:`Class: {bench.Class}`\n:wrench:`Specialization: {bench.Specialization}`\n:bar_chart:`DPS: {bench.DPS}`\n:page_facing_up:`Scale: {bench.Scale}`\n:bookmark_tabs:`DPS(with scale applied): {bench.GetConvertedDPS(true)}`\n\n";
                }
                allContent.Add(new KeyValuePair<string, string>(_roleTitles[group.Key], content));
            }

            return allContent;
        }

        private static List<KeyValuePair<string, string>> GetAllContent(ICollection<IRaidRecord> raidRecords)
        {
            var allContent = new List<KeyValuePair<string, string>>();
            var grouping = raidRecords.GroupBy(x => x.Role);
            foreach(var group in grouping)
            {
                /*string content = "";
                foreach(var record in group)
                {
                    content += $"{record.ID} | {record.CharacterName} | {record.Role} | {record.Class} | {record.Specialization} | {record.DPS}\n";
                }
                content += "";*/
                string content = string.Empty;
                foreach(var record in group)
                {
                    content += $":pushpin:`ID: {record.ID}`\n:clipboard:`Name: {record.CharacterName}`\n:tools:`Role: {record.Role}`\n:hammer:`Class: {record.Class}`\n:wrench:`Specialization: {record.Specialization}`\n";
                    if(record.DPS > 0)
                    {
                        content += $":bar_chart:`DPS: {record.GetConvertedDPS()}`\n";
                    }
                    if(record.BoonUptime > 0)
                    {
                        content += $":clock6:`Boon uptime: {record.BoonUptime}%`\n";
                    }
                    content += $"\n{_benchEmoji[record.Status]}`Status: {record.Status}`\n\n";
                    //content += $":pushpin:`ID: {record.ID}`\n:clipboard:`Name: {record.CharacterName}`\n:tools:`Role: {record.Role}`\n:hammer:`Class: {record.Class}`\n:wrench:`Specialization: {record.Specialization}`\n:bar_chart:`DPS: {record.GetConvertedDPS()}`\n\n{_benchEmoji[record.Status]}`Status: {record.Status}`\n\n";
                }
                allContent.Add(new KeyValuePair<string, string>(_roleTitles[group.Key], content));
            }
            return allContent;
        }
    }
}

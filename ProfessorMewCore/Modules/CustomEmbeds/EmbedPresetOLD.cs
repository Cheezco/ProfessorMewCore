using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.CustomEmbeds
{
    public class EmbedPresetOLD
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int[] RGB { get; set; }
        public string ImageUrl { get; set; }
        public bool ShowCurrentTimestamp { get; set; }
        public string FooterText { get; set; }
        public string FooterIcon { get; set; }
        public string Thumbnail { get; set; }
        public List<EmbedFieldBuilder> EmbedFieldBuilders { get; set; }

        private static readonly string _path = $"{AppDomain.CurrentDomain.BaseDirectory}Data/EmbedPresets/"; 

        private Discord.Color Color
        {
            get
            {
                if (RGB is null || RGB.Length < 3 || RGB.Length > 3)
                {
                    return Discord.Color.Default;
                }

                return new Discord.Color(RGB[0], RGB[1], RGB[2]);
            }
        }

        public EmbedPresetOLD(string title, string description, int[] rgb, string imageUrl, bool showCurrentTimestamp, string footerText, string footerIcon, string thumbnail, List<EmbedFieldBuilder> embedFieldBuilders)
        {
            Title = title;
            Description = description;
            RGB = rgb;
            ImageUrl = imageUrl;
            ShowCurrentTimestamp = showCurrentTimestamp;
            EmbedFieldBuilders = embedFieldBuilders;
            FooterText = footerText;
            FooterIcon = footerIcon;
            Thumbnail = thumbnail;
        }

        public Embed ConstructEmbed()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(Title)
                .WithDescription(Description)
                .WithColor(Color)
                .WithImageUrl(ImageUrl);
            if (!string.IsNullOrEmpty(FooterText))
            {
                if (!string.IsNullOrEmpty(FooterIcon))
                {
                    embedBuilder.WithFooter(FooterText, FooterIcon);
                }
                else
                {
                    embedBuilder.WithFooter(FooterText);
                }
            }
            if (ShowCurrentTimestamp)
            {
                embedBuilder
                    .WithCurrentTimestamp();
            }
            if(!string.IsNullOrEmpty(Thumbnail))
            {
                embedBuilder
                    .WithThumbnailUrl(Thumbnail);
            }
            EmbedFieldBuilders.ForEach(x => embedBuilder.AddField(x.Name, x.Value, x.IsInline));

            return embedBuilder.Build();
        }

        public static EmbedPreset LoadPreset(string fileName)
        {
            if (!PresetExists(fileName)) return null;

            string filePath = $"{_path}{fileName}.json";
            return JsonConvert.DeserializeObject<EmbedPreset>(File.ReadAllText(filePath));
        }
        public static bool PresetExists(string fileName)
        {
            string filePath = $"{_path}{fileName}.json";
            return File.Exists(filePath);
        }
        public static List<string> GetPresetNames()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($"{AppDomain.CurrentDomain.BaseDirectory}Data/EmbedPresets/");
            var fileNames = new List<string>();
            directoryInfo.GetFiles()
                .ToList()
                .ForEach(x => fileNames.Add(x.Name.Substring(0, x.Name.LastIndexOf('.'))));

            return fileNames;
        }
    }
}

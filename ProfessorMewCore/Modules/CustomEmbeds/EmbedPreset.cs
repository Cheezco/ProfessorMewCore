using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProfessorMewCore.Modules.CustomEmbeds
{
    public class EmbedPreset
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("color")]
        public int Color { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("footer")]
        public PresetFooter Footer { get; set; }
        [JsonProperty("thumbnail")]
        public PresetThumbnail Thumbnail { get; set; }
        [JsonProperty("image")]
        public PresetImage Image { get; set; }
        [JsonProperty("fields")]
        public List<EmbedFieldBuilder> Fields { get; set; }

        private static readonly string _path = $"{AppDomain.CurrentDomain.BaseDirectory}Data/EmbedPresets/";
        private bool WithCurrentTimestamp 
        {
            get
            {
                if (string.IsNullOrEmpty(Timestamp)) return false;
                return true;
            }
        }

        private Discord.Color DiscordColor
        {
            get
            {
                return new Discord.Color((Color >> 16) & 255, (Color >> 8) & 255, Color & 255);
            }
        }

        public Embed ConstructEmbed()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(Title)
                .WithUrl(Url)
                .WithDescription(Description)
                .WithColor(DiscordColor);
            if(Image is not null)
            {
                embedBuilder
                    .WithImageUrl(Image.Url);
            }
            if(WithCurrentTimestamp)
            {
                embedBuilder
                    .WithCurrentTimestamp();
            }
            if(Footer is not null)
            {
                embedBuilder
                    .WithFooter(Footer.Text, Footer.IconUrl);
            }
            if(Thumbnail is not null)
            {
                embedBuilder
                    .WithThumbnailUrl(Thumbnail.Url);
            }
            if(Fields is not null || Fields.Count > 0)
            {
                embedBuilder.Fields.AddRange(Fields);
            }

            return embedBuilder.Build();
        }

        public static EmbedPreset LoadPreset(string fileName)
        {
            string filePath = $"{_path}{fileName}.json";
            return ParsePreset(File.ReadAllText(filePath));
        }
        public static EmbedPreset ParsePreset(string content)
        {
            return JsonConvert.DeserializeObject<EmbedPreset>(content);
        }
        public void SavePreset(string fileName)
        {
            if (PresetExists(fileName)) return;

            string filePath = $"{_path}{fileName}.json";
            var output = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, output);
        }
        public static bool PresetExists(string fileName)
        {
            string filePath = $"{_path}{fileName}.json";
            return File.Exists(filePath);
        }
        public static List<string> GetPresetNames()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_path);
            var fileNames = new List<string>();
            directoryInfo.GetFiles()
                .ToList()
                .ForEach(x => fileNames.Add(x.Name.Substring(0, x.Name.LastIndexOf('.'))));

            return fileNames;
        }
    }

    public class PresetFooter
    {
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class PresetThumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class PresetImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

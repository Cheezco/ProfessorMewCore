using ImageCreator;
using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using HtmlAgilityPack;
using ProfessorMewData.Enums.Raid;
using System.Diagnostics;

namespace ProfessorMewCore.Modules.Raid
{
    public static class RaidProfile
    {
        private static readonly string _assetPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Assets/";
        private static readonly string _tempPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Temp/";

        public static async Task<string> CreateRaidProfileImageAsync(IRaidUser user, string profilePicLocation)
        {
            await PrepareHtml(user, profilePicLocation);

            var process = Process.Start("Data/wkhtmltoimage.exe", $"{_assetPath}RaidProfile/{user.DBDiscordID}.html {_tempPath}{user.DBDiscordID}_image.jpg");
            await process.WaitForExitAsync();

            File.Delete($"{_assetPath}RaidProfile/{user.DBDiscordID}.html");
            File.Delete($"{_tempPath}{user.DBDiscordID}.jpg");
            return $"{_tempPath}{user.DBDiscordID}_image.jpg";
        }
        
        public static async Task PrepareHtml(IRaidUser user, string profilePicLocation)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(await File.ReadAllTextAsync($"{_assetPath}RaidProfile/RaidProfile.html"));
            doc.GetElementbyId("Username").InnerHtml = user.AccountName;
            doc.GetElementbyId("profileImage").SetAttributeValue("src", profilePicLocation);

            foreach(var record in user.Records)
            {
                string elementId = string.Empty;

                switch(record.Role)
                {
                    case Role.Alac or
                         Role.Boon or
                         Role.Quickness or
                         Role.Tank or
                         Role.Healer:
                        elementId = "SUPPORT_";
                        break;
                    case Role.Condi or
                         Role.Power or
                         Role.Bannerslave:
                        elementId = "DPS_";
                         break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(elementId) || record.Status != BenchStatus.Passed) continue;

                if(record.Specialization == Specialization.Base)
                {
                    elementId += Enum.GetName(typeof(Class), record.Class);
                }
                else
                {
                    elementId += Enum.GetName(typeof(Specialization), record.Specialization);
                }

                var element = doc.GetElementbyId(elementId);

                if (element is null) continue;

                element.SetAttributeValue("class", "icon");
            }

            using(var writer = File.Create($"{_assetPath}RaidProfile/{user.DBDiscordID}.html"))
            {
                doc.Save(writer);
            }
        }
    }
}

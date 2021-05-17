using ProfessorMewData.Interfaces.Raid;
using System;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ProfessorMewData.Enums.Raid;
using Svg.Skia;

namespace ProfessorMewCore.Modules.Raid
{
    public static class RaidProfile
    {
        private static readonly string _assetPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Assets/";
        private static readonly string _tempPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Temp/";

        public static async Task<string> CreateRaidProfileImageAsync(IRaidUser user, string profilePicPath)
        {
            await PrepareSvgAsync(user, profilePicPath);

            using(var svg = new SKSvg())
            {
                svg.Load($"{_assetPath}RaidProfile/{user.DiscordID}.svg");
                using (var stream = File.OpenWrite($"{_tempPath}{user.DBDiscordID}_image.jpeg"))
                {
                    svg.Picture.ToImage(stream, SkiaSharp.SKColor.Empty, SkiaSharp.SKEncodedImageFormat.Jpeg, 75, 1f, 1f, SkiaSharp.SKImageInfo.PlatformColorType, SkiaSharp.SKAlphaType.Unpremul, SkiaSharp.SKColorSpace.CreateRgb(SkiaSharp.SKColorSpaceTransferFn.Srgb, SkiaSharp.SKColorSpaceXyz.Srgb));
                }
            }

            File.Delete($"{_assetPath}RaidProfile/{user.DBDiscordID}.svg");
            File.Delete($"{_tempPath}{user.DBDiscordID}.jpg");

            return $"{_tempPath}{user.DBDiscordID}_image.jpeg";
        }
        
        public static async Task PrepareSvgAsync(IRaidUser user, string profilePicPath)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(await File.ReadAllTextAsync($"{_assetPath}RaidProfile/RaidProfile.svg"));
            doc.GetElementbyId("Name").InnerHtml = user.AccountName;
            doc.GetElementbyId("testingProfileImage").SetAttributeValue("href", profilePicPath);

            foreach (var record in user.Records)
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

                if (record.Specialization == Specialization.Base)
                {
                    elementId += Enum.GetName(typeof(Class), record.Class);
                }
                else
                {
                    elementId += Enum.GetName(typeof(Specialization), record.Specialization);
                }

                var element = doc.GetElementbyId(elementId);

                if (element is null) continue;

                element.SetAttributeValue("class", "cls-12");
            }

            string path = $"{_assetPath}RaidProfile/{user.DBDiscordID}.svg";

            using (var writer = File.CreateText(path))
            {
                doc.Save(writer);
            }

            string fixedFile = File.ReadAllText(path)
                .Replace("preserveaspectratio", "preserveAspectRatio")
                .Replace("viewbox", "viewBox");
            File.WriteAllText(path, fixedFile);
        }
    }
}

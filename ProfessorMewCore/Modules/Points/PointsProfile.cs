using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageCreator.Elements;
using ImageCreator;
using ProfessorMewData.Interfaces.Guild;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using System.IO;

namespace ProfessorMewCore.Modules.Points
{
    public static class PointsProfile
    {
        private static readonly string _imagePath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Images/";
        private static readonly string _assetPath = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Assets/";
        public static async Task<string> CreateProfileImage(IUser user, Stream profilePicture)
        {
            var imageData = await GetProfileImageData(user, profilePicture);
            var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();

            using var background = await Image.LoadAsync(imageData.BackgroundPath);
            background.Mutate(x => x.Resize(imageData.Size)
                .DrawContainers(imageData.ImageContainers));
            

            await background.SaveAsync($"{_imagePath}{user.DiscordID}.jpg");

            return $"{_imagePath}{user.DiscordID}.jpg";
        }

        private static async Task<ImageData> GetProfileImageData(IUser user, Stream profilePicture)
        {
            var fontCollection = new FontCollection();
            var fontFamilty = fontCollection.Install($"{_assetPath}Fonts/Montserrat-Regular.ttf");
            var font = fontFamilty.CreateFont(40, FontStyle.Regular);
            var profileFont = fontFamilty.CreateFont(32, FontStyle.Regular);

            var totalPointsLabel = new ImageCreator.Elements.Label
            {
                IconPath = $"{_assetPath}Images/TotalPointsIcon.png",
                Text = $"Total Points: {user.TotalPoints}",
                Size = new Size(51, 51),
                Font = font,
                TextColor = Color.White,
                TextOffset = new Point(60, -6),
            };

            var monthPointsLabel = new ImageCreator.Elements.Label
            {
                IconPath = $"{_assetPath}Images/MonthPointsIcon.png",
                Text = $"Month Points: {user.MonthPoints}",
                Size = new Size(51, 51),
                Font = font,
                TextColor = Color.White,
                TextOffset = new Point(60, -6),
            };

            var rankingLabel = new ImageCreator.Elements.Label
            {
                IconPath = $"{_assetPath}Images/RankingIcon.png",
                Text = $"Ranking: {user.GuildRanking}",
                Size = new Size(51, 51),
                Font = font,
                TextColor = Color.White,
                TextOffset = new Point(60, -6),
            };

            var rankLabel = new ImageCreator.Elements.Label
            {
                IconPath = $"{_assetPath}Images/RankIcon.png",
                Text = $"Rank: {user.Rank.Name}",
                Size = new Size(51, 51),
                Font = font,
                TextColor = Color.White,
                TextOffset = new Point(60, -6),
            };

            var profileImg = await Image.LoadAsync(profilePicture);
            using (var rankImg = new Image<Rgba32>(200, 200))
            {
                var color = System.Drawing.ColorTranslator.FromHtml(user.Rank.ColorCode);
                rankImg.Mutate(x => x.Fill(Brushes.Solid(Color.FromRgb(color.R, color.G, color.B)), new RectangularPolygon(0, 0, 200, 200))
                    .ConvertToAvatar(new Size(60, 60), 30));
                profileImg.Mutate(x => x.ConvertToAvatar(new Size(240, 240), 125)
                    .CutOutEllipse(200, 200, 40));
                profileImg.Mutate(x => x.DrawImage(rankImg, new Point(170, 170), 1f));
            }

            string username = user.Name.Length > 15 ? user.Name.Substring(0, 15) : user.Name;

            var profilePictureLabel = new ImageCreator.Elements.Label
            {
                Text = $"{username}",
                Size = new Size(240, 240),
                Font = profileFont,
                TextColor = Color.White,
                TextOffset = new Point(125, 230),
                PreloadedIcon = profileImg,
                TextAlignment = HorizontalAlignment.Center
            };

            var labelContainer = new ImageContainer
            {
                Elements = new List<ImageElement>
                {
                    totalPointsLabel,
                    monthPointsLabel,
                    rankingLabel,
                    rankLabel
                },
                Position = new Point(390, 14),
                Size = new Size(605, 341),
                Opacity = 0.6f,
                CornerRadius = 25,
                Layout = ImageContainer.ImageContainerLayout.Horizontal,
                Brush = Brushes.Solid(Color.FromRgb(75, 75, 75)),
                LabelOffset = new Point(25, 20)
            };

            var mainContainer = new ImageContainer
            {
                Containers = new List<ImageContainer>
                {
                    labelContainer
                },
                Elements = new List<ImageElement>
                {
                    profilePictureLabel
                },
                Position = new Point(35, 15),
                Size = new Size(960, 340),
                Opacity = 0.4f,
                CornerRadius = 25,
                Layout = ImageContainer.ImageContainerLayout.Vertical,
                Brush = Brushes.Solid(Color.FromRgb(75, 75, 75)),
                LabelOffset = new Point(60, 10)
            };

            return new ImageData
            {
                BackgroundPath = $"{_assetPath}Images/background3.png",
                ImageContainers = new List<ImageContainer>
                {
                    mainContainer
                },
                Size = new Size(1024, 376)
            };
        }
    }
}

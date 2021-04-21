using ProfessorMewData.Interfaces.Guild;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewCore.Modules.Points
{
    class ProfileImageOLD : IDisposable
    {
        //private readonly Size avatarSize = new Size(150, 150);

        //private readonly Point nameStart = new Point(190, 200);

        private readonly Point labelStart = new Point(25, 200);
        private readonly int labelPadding = 40;

        private readonly string path = $"{AppDomain.CurrentDomain.BaseDirectory}Data/Images/";

        private Point avatarStart;
        private Point center;
        private Image avatar;
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;
        private readonly Font font;
        private readonly IUser user;
        public string FontName;
        public List<Label> labels;

        public ProfileImageOLD(IUser plr, Bitmap background = null, string fontName = "Trench")
        {
            if (background == null)
            {
                bitmap = new Bitmap(Image.FromFile(path + "/avatar_template.png"));
            }
            else
            {
                bitmap = background;
            }
            //this.bitmap = new Bitmap(Image.FromFile(path + "/avatar_template.png"));
            graphics = Graphics.FromImage(bitmap);
            //this.font = new Font("Constantia", 18);
            FontName = fontName;
            font = new Font(FontName, 20, FontStyle.Bold);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            center = new Point(bitmap.Width / 2, bitmap.Height / 2);

            user = plr;
        }

        private void DrawLabels(Brush brush)
        {
            int i = 0;
            foreach (Label label in labels)
            {
                int y = labelStart.Y + (labelPadding * i);

                graphics.DrawImage(new Bitmap(label.Icon, new Size((int)font.Size, (int)font.Size)), labelStart.X - font.Size, y + 5);

                graphics.DrawString(label.Key, font, brush, labelStart.X, y);
                graphics.DrawString(label.Value, font, brush, labelStart.X + 200, y);

                i++;
            }
        }

        private void DrawBorder(Color color, int width)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddPolygon(new Point[]
            {
                    new Point(-1, -1),
                    new Point(bitmap.Size.Width, -1),
                    new Point( bitmap.Size.Width, bitmap.Size.Height),
                    new Point(-1, bitmap.Size.Height),
            });
            Pen pen = new Pen(color, width);
            graphics.DrawPath(pen, graphicsPath);

            //pen.Dispose();
            //graphics.Dispose();
        }

        /*private Dictionary<string, string> GetLabels()
        {
            var labels = new Dictionary<string, string>
            {
                { "Total Points", user.TotalPoints + " / " + user.Rank.MaxPoints },
                { "Monthly Points", user.MonthPoints.ToString(CultureInfo.InvariantCulture) },
                { "Ranking", user.GuildRanking.ToString(CultureInfo.InvariantCulture) },
                { "Rank", user.Rank.Name }
            };

            return labels;
        }*/
        private void DrawAvatar()
        {
            avatar = ImageUtils.ImageFromUrl(user.AvatarUrl);
            Image roundedAvatar = ImageUtils.RoundImage(avatar);
            avatarStart = new Point(center.X - (avatar.Width / 2), 10);

            graphics.DrawImage(roundedAvatar, avatarStart);
            var color = ColorTranslator.FromHtml(user.Rank.ColorCode);
            DrawRoundBorder(avatarStart, avatar.Size, color, 3);//Color.FromArgb(240, 240, 240), 2);
            roundedAvatar.Dispose();
        }

        private void DrawIcon()
        {
            Size iconSize = new Size(60, 60);
            Bitmap icon = new Bitmap(Image.FromFile(path + "/GuildIcon2.png"), iconSize);
            Point iconStart = new Point(bitmap.Width - (iconSize.Width + 10), bitmap.Height - (iconSize.Height + 10));
            graphics.DrawImage(icon, iconStart);

            DrawRoundBorder(iconStart, icon.Size, Color.FromArgb(240, 240, 240), 2);
            icon.Dispose();
        }

        private void DrawName(Brush brush)
        {

            string name = user.Name.Length <= 10 ? user.Name : user.Name.Substring(0, 10);

            int opacity = 75;

            Point point = new Point(avatarStart.X, avatarStart.Y + avatar.Size.Width + 15);
            Size size = new Size(avatar.Width, 25);

            using (SolidBrush nameFieldBrush = new SolidBrush(Color.FromArgb(opacity, 0, 0, 0)))
            {
                Bitmap nameBitmap = new Bitmap(size.Width, size.Height);

                using (Graphics graph = Graphics.FromImage(nameBitmap))
                {
                    graph.Clear(nameFieldBrush.Color);
                    using (Font nameFont = new Font(FontName, 16, FontStyle.Bold))//new Font("Constantia", 14))
                    {
                        StringFormat stringFormat = new StringFormat() { Alignment = StringAlignment.Center };
                        graph.DrawString(name, nameFont, brush, new Rectangle(new Point(0, 3), size), new StringFormat() { Alignment = StringAlignment.Center });

                        //stringFormat.Dispose();
                    }
                }
                graphics.DrawImage(nameBitmap, point);
                // nameBitmap.Dispose();

            }
        }

        private void DrawRoundBorder(Point point, Size size, Color color, int width)
        {

            using (GraphicsPath graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddEllipse(new Rectangle(point, size));
                Pen pen = new Pen(color, width);
                graphics.DrawPath(pen, graphicsPath);
                pen.Dispose();
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static EncoderParameters EncoderParameters()
        {
            var qualityParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 75L);
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = qualityParameter;
            return parameters;
        }

        private string Save(string filename)
        {
            EncoderParameters encoderParameters = EncoderParameters();
            bitmap.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
            encoderParameters.Dispose();

            return filename;
        }
        private Stream Save(Stream stream)
        {
            EncoderParameters encoderParameters = EncoderParameters();
            bitmap.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
            encoderParameters.Dispose();

            return stream;
        }
        private void Draw()
        {
            using (var brush = new SolidBrush(Color.FromArgb(240, 240, 240)))
            {

                DrawLabels(brush);
                DrawAvatar();
                //this.DrawBadges(brush, true);
                DrawBorder(ColorTranslator.FromHtml(user.Rank.ColorCode), 6);
                DrawName(brush);
                DrawIcon();
            }
        }
        public void Render(Stream stream)
        {
            Draw();
            Save(stream);
        }

        public void Render(string filepath)
        {
            Draw();
            Save(filepath);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    font.Dispose();
                    bitmap.Dispose();
                    graphics.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
    class Label
    {
        public Image Icon { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    static class ImageUtils
    {
        public static Image ImageFromUrl(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            return Image.FromStream(stream);
        }

        public static Image RoundImage(Image image)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(0, 0, image.Width, image.Height);

                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.SetClip(gp);
                    gr.DrawImage(image, Point.Empty);
                }
            }

            return bmp;
        }
    }
}

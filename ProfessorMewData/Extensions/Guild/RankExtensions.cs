using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewData.Extensions.Guild
{
    public static class RankExtensions
    {
        public static Discord.Color GetDiscordColor(this IRank rank)
        {
            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(rank.ColorCode);
            return new Discord.Color(color.R, color.G, color.B);
        }
    }
}

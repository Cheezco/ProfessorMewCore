using Xunit;
using FluentAssertions;
using ProfessorMewData.Models.Guild;
using ProfessorMewData.Extensions.Guild;

namespace ProfessorMewTests
{
    public class RankExtensionsTests
    {
        [Fact]
        public void GetDiscordColor_RedColor()
        {
            var rank = new Rank()
            {
                ColorCode = "#ff0000"
            };
            var color = rank.GetDiscordColor();
            var colorToCompare = new Discord.Color(255, 0, 0);
            color.Should().Be(colorToCompare);
        }
        [Fact]
        public void GetDiscordColor_Purple()
        {
            var rank = new Rank()
            {
                ColorCode = "#9134b3"
            };
            var color = rank.GetDiscordColor();
            var colorToCompare = new Discord.Color(145, 52, 179);
            color.Should().Be(colorToCompare);
        }
        [Fact]
        public void GetDiscordColor_White()
        {
            var rank = new Rank()
            {
                ColorCode = "#ffffff"
            };
            var color = rank.GetDiscordColor();
            var colorToCompare = new Discord.Color(255, 255, 255);
            color.Should().Be(colorToCompare);
        }
    }
}

using Xunit;
using FluentAssertions;
using ProfessorMewData.Models.Raid;
using ProfessorMewData.Extensions.Raid;

namespace ProfessorMewTests
{
    public class RaidBenchExtensionsTests
    {
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandNoScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = 1
            };
            raidBench.GetConvertedDPS().Should().Be("1");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandNoScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 100
            };
            raidBench.GetConvertedDPS().Should().Be("100");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandNoScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 999
            };
            raidBench.GetConvertedDPS().Should().Be("999");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandWithScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = 1,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("0");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandWithScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 100,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("80");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanThousandWithScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 999,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("799");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionNoScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = 1000
            };
            raidBench.GetConvertedDPS().Should().Be("1K");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionNoScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 9999
            };
            raidBench.GetConvertedDPS().Should().Be("9.999K");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionNoScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 5896
            };
            raidBench.GetConvertedDPS().Should().Be("5.896K");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionWithScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = 5000,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("4K");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionWithScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 99999,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("79.999K");
        }
        [Fact]
        public void GetConvertedDPS_DPSLessThanMillionWithScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 485687,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be("388.549K");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionNoScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = int.MaxValue
            };
            raidBench.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionNoScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 1000000
            };
            raidBench.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionNoScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 85623419
            };
            raidBench.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionScale()
        {
            var raidBench = new RaidBench()
            {
                DPS = 85623419,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionScale2()
        {
            var raidBench = new RaidBench()
            {
                DPS = 9999999,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_DPSMoreThanMillionScale3()
        {
            var raidBench = new RaidBench()
            {
                DPS = 7854685,
                Scale = 0.8
            };
            raidBench.GetConvertedDPS(true).Should().Be(">=1M");
        }
    }
}

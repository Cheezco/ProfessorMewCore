using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using ProfessorMewData.Models.Raid;
using ProfessorMewData.Extensions.Raid;
using ProfessorMewData.Interfaces.Raid;
using ProfessorMewData.Enums.Raid;

namespace ProfessorMewTests
{
    public class RaidRecordExtensionsTests
    {
        [Fact]
        public void GetConvertedDPS_NumberLessThanThousand()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 1
            };
            raidRecord.GetConvertedDPS().Should().Be("1");
        }
        [Fact]
        public void GetConvertedDPS_NumberLessThanThousand2()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 100
            };
            raidRecord.GetConvertedDPS().Should().Be("100");
        }
        [Fact]
        public void GetConvertedDPS_NumberLessThanThousand3()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 999
            };
            raidRecord.GetConvertedDPS().Should().Be("999");
        }
        [Fact]
        public void GetConvertedDPS_NumberLessThanMillion()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 1000
            };
            raidRecord.GetConvertedDPS().Should().Be("1K");
        }
        [Fact]
        public void GetConvertedDPS_NumberLessThanMillion2()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 9999
            };
            raidRecord.GetConvertedDPS().Should().Be("9.999K");
        }
        [Fact]
        public void GetConvertedDPS_NumberLessThanMillion3()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 500000
            };
            raidRecord.GetConvertedDPS().Should().Be("500K");
        }
        [Fact]
        public void GetConvertedDPS_NumberMoreThanMillion()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 1000000
            };
            raidRecord.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_NumberMoreThanMillion2()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = int.MaxValue
            };
            raidRecord.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void GetConvertedDPS_NumberMoreThanMillion3()
        {
            var raidRecord = new RaidRecord()
            {
                DPS = 99999999
            };
            raidRecord.GetConvertedDPS().Should().Be(">=1M");
        }
        [Fact]
        public void UpdateRecordStatus_BenchesIsNull_BenchStatusUnkown()
        {
            var record = new RaidRecord();
            record.UpdateRecordStatus(null);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Unkown);
        }
        [Fact]
        public void UpdateRecordStatus_BenchNotFound_BenchStatusUnkown()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Mesmer,
                Specialization = Specialization.Base,
                Role = Role.Alac
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Unkown);
        }
        [Fact]
        public void UpdateRecordStatus_BenchNotFound_BenchStatusUnkown2()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Elementalist,
                Specialization = Specialization.Dragonhunter,
                Role = Role.Alac
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Unkown);
        }
        [Fact]
        public void UpdateRecordStatus_BenchNotFound_BenchStatusUnkown3()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Elementalist,
                Specialization = Specialization.Base,
                Role = Role.Bannerslave
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Unkown);
        }
        [Fact]
        public void UpdateRecordStatus_LowDPS_BenchStatusFailed()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 100,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 1
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Failed);
        }
        [Fact]
        public void UpdateRecordStatus_LowDPS_BenchStatusFailed2()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 15000,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5000
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Failed);
        }
        [Fact]
        public void UpdateRecordStatus_LowBoonUptime_BenchStatusFailed()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 100,
                    BoonUptime = 100,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5000,
                BoonUptime = 5
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Failed);
        }
        [Fact]
        public void UpdateRecordStatus_LowBoonUptime_BenchStatusFailed2()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 100,
                    BoonUptime = 25,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5000,
                BoonUptime = 24
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Failed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodDPS_BenchStatusPassed()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 100,
                    BoonUptime = 0,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5000,
                BoonUptime = 0
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodDPS_BenchStatusPassed2()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 25000,
                    BoonUptime = 0,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 30000,
                BoonUptime = 0
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodDPS_BenchStatusPassed3()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 1,
                    BoonUptime = 0,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5,
                BoonUptime = 0
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodBoonUptime_BenchStatusPassed()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 1,
                    BoonUptime = 25,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5,
                BoonUptime = 26
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodBoonUptime_BenchStatusPassed2()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 1,
                    BoonUptime = 100,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5,
                BoonUptime = 100
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
        [Fact]
        public void UpdateRecordStatus_GoodBoonUptime_BenchStatusPassed3()
        {
            var benches = new List<IRaidBench>()
            {
                new RaidBench()
                {
                    Class = Class.Elementalist,
                    Specialization = Specialization.Base,
                    Role = Role.Alac,
                    Scale = 0.8
                },
                new RaidBench()
                {
                    Class = Class.Engineer,
                    Specialization = Specialization.Holosmith,
                    Role = Role.Power,
                    DPS = 1,
                    BoonUptime = 1,
                    Scale = 0.8
                },
            };
            var record = new RaidRecord()
            {
                Class = Class.Engineer,
                Specialization = Specialization.Holosmith,
                Role = Role.Power,
                DPS = 5,
                BoonUptime = 2
            };
            record.UpdateRecordStatus(benches);
            (record as IRaidRecord).Status.Should().BeEquivalentTo(BenchStatus.Passed);
        }
    }
}

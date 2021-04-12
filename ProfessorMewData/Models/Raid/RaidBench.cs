using ProfessorMewData.Enums.Raid;
using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Models.Raid
{
    public class RaidBench : IRaidBench
    {
        public long ID { get; set; }

        public Role Role { get; set; }

        public Class Class { get; set; }

        public Specialization Specialization { get; set; }

        public int DPS { get; set; }

        public double BoonUptime { get; set; }

        public double Scale { get; set; }

        public IRaidGuild Guild { get; set; }
    }
}

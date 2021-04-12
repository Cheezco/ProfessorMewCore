using ProfessorMewData.Enums.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Interfaces.Raid
{
    public interface IRaidBench
    {
        long ID { get; }
        Role Role { get; }
        Class Class { get; }
        Specialization Specialization { get; }
        int DPS { get; }
        double BoonUptime { get; set; }
        double Scale { get; }
        IRaidGuild Guild { get; }
    }
}

using ProfessorMewData.Enums.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Interfaces.Raid
{
    public interface IRaidRecord
    {
        long ID { get; }
        string CharacterName { get; }
        IRaidUser User { get; }
        Class Class { get; }
        Specialization Specialization { get; }
        Role Role { get; }
        int DPS { get; }
        double BoonUptime { get; }
        BenchStatus Status { get; set; }
    }
}

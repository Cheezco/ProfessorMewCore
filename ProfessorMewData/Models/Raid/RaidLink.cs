using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Models.Raid
{
    public class RaidLink : IRaidLink
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public IRaidGuild Guild { get; set; }
    }
}

using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Models.Raid
{
    public class RaidGuild : Guild.SavedEntity, IRaidGuild
    {
        public ICollection<IRaidUser> Users { get; set; }

        public ICollection<IRaidBench> Benches { get; set; }

        public ICollection<IRaidLink> Links { get; set; }
    }
}

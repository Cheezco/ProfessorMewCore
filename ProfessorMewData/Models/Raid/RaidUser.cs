using ProfessorMewData.Interfaces.Raid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Models.Raid
{
    public class RaidUser : Guild.SavedEntity, IRaidUser
    {
        public string AccountName { get; set; }

        public ICollection<IRaidRecord> Records { get; set; }
        public IRaidGuild Guild { get; set; }
    }
}

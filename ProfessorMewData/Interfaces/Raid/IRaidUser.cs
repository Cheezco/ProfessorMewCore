using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Interfaces.Raid
{
    public interface IRaidUser : Guild.ISavedEntity
    {
        string AccountName { get; }
        ICollection<IRaidRecord> Records { get; set; }
        IRaidGuild Guild { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Interfaces.Raid
{
    public interface IRaidGuild : Guild.ISavedEntity
    {
        ICollection<IRaidUser> Users { get; }
        ICollection<IRaidBench> Benches { get; }
        ICollection<IRaidLink> Links { get; }
    }
}

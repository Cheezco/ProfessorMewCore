using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfessorMewData.Interfaces.Raid
{
    public interface IRaidLink
    {
        long ID { get; }
        string Name { get; }
        string URL { get; }
        IRaidGuild Guild { get; }
    }
}

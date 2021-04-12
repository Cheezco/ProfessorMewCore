using ProfessorMewData.Interfaces.Guild;
using System.Collections.Generic;

namespace ProfessorMewData.Models.Guild
{
    public class Rank : SavedEntity, IRank
    {
        public string Name { get; set; }

        public string ColorCode { get; set; }

        public int MinPoints { get; set; }

        public int MaxPoints { get; set; }

        public int RankOrder { get; set; }

        public bool Default { get; set; }

        public ISavedGuild Guild { get; set; }

        public ICollection<IUser> Users { get; set; }
    }
}

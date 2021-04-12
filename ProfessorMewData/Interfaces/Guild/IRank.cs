using System.Collections.Generic;

namespace ProfessorMewData.Interfaces.Guild
{
    public interface IRank : ISavedEntity
    {
        string Name { get; }
        string ColorCode { get; }
        int MinPoints { get; }
        int MaxPoints { get; }
        int RankOrder { get; }
        bool Default { get; }

        ISavedGuild Guild { get; }
        ICollection<IUser> Users { get; }
    }
}

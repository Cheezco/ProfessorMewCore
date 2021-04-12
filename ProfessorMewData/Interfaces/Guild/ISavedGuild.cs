using System.Collections.Generic;

namespace ProfessorMewData.Interfaces.Guild
{
    public interface ISavedGuild : ISavedEntity
    {
        string Prefix { get; }
        ICollection<IUser> Users { get; }
        ICollection<IRank> Ranks { get; }
        ICollection<IGuildRole> Roles { get; }
        ICollection<ISavedLink> Links { get; }
        ICollection<ISavedChannel> Channels { get; }
        ICollection<ISavedMessage> Messages { get; }
        ICollection<ILotteryUser> LotteryUsers { get; }
    }
}

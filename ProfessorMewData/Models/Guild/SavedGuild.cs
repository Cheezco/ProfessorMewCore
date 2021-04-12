using ProfessorMewData.Interfaces.Guild;
using System.Collections.Generic;

namespace ProfessorMewData.Models.Guild
{
    public class SavedGuild : SavedEntity, ISavedGuild
    {
        public string Prefix { get; set; }

        public ICollection<IUser> Users { get; set; }

        public ICollection<IRank> Ranks { get; set; }

        public ICollection<IGuildRole> Roles { get; set; }

        public ICollection<ISavedLink> Links { get; set; }

        public ICollection<ISavedChannel> Channels { get; set; }

        public ICollection<ISavedMessage> Messages { get; set; }

        public ICollection<ILotteryUser> LotteryUsers { get; set; }
    }
}

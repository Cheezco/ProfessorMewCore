using ProfessorMewData.Interfaces.Guild;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfessorMewData.Models.Guild
{
    public class LotteryUser : SavedEntity, ILotteryUser
    {
        [NotMapped]
        public string Name { get; set; }

        public int Tickets { get; set; }

        public ISavedGuild Guild { get; set; }

        public long UserID { get; set; }

        public IUser User { get; set; }
    }
}

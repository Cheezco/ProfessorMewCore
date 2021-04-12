
namespace ProfessorMewData.Interfaces.Guild
{
    public interface ILotteryUser : ISavedEntity
    {
        string Name { get; set; }
        int Tickets { get; set; }
        ISavedGuild Guild { get; set; }
        long UserID { get; set; }
        IUser User { get; set; }
    }
}

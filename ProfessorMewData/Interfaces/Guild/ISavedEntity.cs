
namespace ProfessorMewData.Interfaces.Guild
{
    public interface ISavedEntity
    {
        long ID { get; }
        ulong DiscordID { get; }
        string DBDiscordID { get; }
        bool IsEmpty { get; }
    }
}


namespace ProfessorMewData.Interfaces.Guild
{
    public interface IGuildRole : ISavedEntity
    {
        string Name { get; }
        bool AddUserToDatabase { get; }
        ISavedGuild Guild { get; }
    }
}

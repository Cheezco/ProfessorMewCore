
namespace ProfessorMewData.Interfaces.Guild
{
    public interface ISavedChannel : ISavedEntity
    {
        string Name { get; }
        bool ExecuteCommands { get; }
        ISavedGuild Guild { get; }
    }
}

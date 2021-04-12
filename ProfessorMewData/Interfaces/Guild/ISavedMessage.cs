
namespace ProfessorMewData.Interfaces.Guild
{
    public interface ISavedMessage : ISavedEntity
    {
        string Name { get; }
        ISavedGuild Guild { get; }
    }
}

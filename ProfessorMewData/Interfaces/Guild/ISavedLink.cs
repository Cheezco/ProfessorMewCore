
namespace ProfessorMewData.Interfaces.Guild
{
    public interface ISavedLink : ISavedEntity
    {
        string Name { get; }
        string URL { get; }
        ISavedGuild Guild { get; }
    }
}

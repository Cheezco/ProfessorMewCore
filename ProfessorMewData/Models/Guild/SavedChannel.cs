using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewData.Models.Guild
{
    public class SavedChannel : SavedEntity, ISavedChannel
    {
        public string Name { get; set; }

        public bool ExecuteCommands { get; set; }

        public ISavedGuild Guild { get; set; }
    }
}

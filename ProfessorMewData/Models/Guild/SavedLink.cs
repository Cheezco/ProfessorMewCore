using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewData.Models.Guild
{
    public class SavedLink : SavedEntity, ISavedLink
    {
        public string Name { get; set; }

        public string URL { get; set; }

        public ISavedGuild Guild { get; set; }
    }
}

using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewData.Models.Guild
{
    public class SavedMessage : SavedEntity, ISavedMessage
    {
        public string Name { get; set; }

        public ISavedGuild Guild { get; set; }
    }
}

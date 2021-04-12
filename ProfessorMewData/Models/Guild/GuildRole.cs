using ProfessorMewData.Interfaces.Guild;

namespace ProfessorMewData.Models.Guild
{
    public class GuildRole : SavedEntity, IGuildRole
    {
        public string Name { get; set; }

        public bool AddUserToDatabase { get; set; }

        public ISavedGuild Guild { get; set; }
    }
}

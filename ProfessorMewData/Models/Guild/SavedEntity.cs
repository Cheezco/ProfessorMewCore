using ProfessorMewData.Interfaces.Guild;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfessorMewData.Models.Guild
{
    public class SavedEntity : ISavedEntity
    {
        public long ID { get; set; }
        [NotMapped]
        public ulong DiscordID
        {
            get
            {
                return Convert.ToUInt64(DBDiscordID);
            }
            set
            {
                DBDiscordID = value.ToString();
            }
        }

        public string DBDiscordID { get; set; }

        public bool IsEmpty => ID == 0;
    }
}

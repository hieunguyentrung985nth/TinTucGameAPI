using System.ComponentModel.DataAnnotations.Schema;

namespace TinTucGameAPI.Models2
{
    public class Participants
    {
        public Room Room { get; set; }
        public string RoomId { get; set; }
        public string UserId { get; set; }
        public staff User { get; set; }
    }
}

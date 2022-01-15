using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestbook.Models
{
    public class UserMessage
    {
        public int UserId { get; set; }
        public int ReceivingUserId { get; set; }
        public int MessId { get; set; }
        
        public virtual User Users { get; set; }
        public virtual Message Messages { get; set; }
    }
}

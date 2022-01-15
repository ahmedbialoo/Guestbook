using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestbook.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessId { get; set; }
        [Required(ErrorMessage ="The Message field is required")]
        [StringLength(500)]
        [Display(Name = "Write a Message")]
        public string messBody { get; set; }
        [StringLength(500)]
        [Display(Name = "Write a Response")]
        public string? respnose { get; set; }
        [Display(Name = "Published Dated")]
        public DateTime publishedDate { get; set; }
        public virtual List<UserMessage> UserMessages { get; set; }
    }
}

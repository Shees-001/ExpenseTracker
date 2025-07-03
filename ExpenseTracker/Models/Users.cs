using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ExpenseTracker.Models
{
    public class Users
    {
        [Key]
        public int User_Id { get; set; }
        [Required]
        public string User_Name { get; set; }
        [Required]
        public string User_Email { get; set; }
        [Required]
        public string User_Password { get; set; }
        public string? OTP { get; set; }
        [DefaultValue(0)]
        public int User_Role { get; set; }
        [NotMapped]
        public string? face_image { get; set; }

    }
}

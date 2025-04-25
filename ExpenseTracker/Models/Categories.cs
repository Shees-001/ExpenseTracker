using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{
    public class Categories
    {
        [Key]
        public int Category_Id { get; set; }
        [Required]
        public string Category_Name { get; set; }
    }
}

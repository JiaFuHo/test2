using System.ComponentModel.DataAnnotations;

namespace test2.Models
{
    public class PasswordC
    {
        [Key]
        public string cId { get; set; } = string.Empty;
        public string cPassword { get; set; } = string.Empty;
    }
}
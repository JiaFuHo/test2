using System.ComponentModel.DataAnnotations;

namespace test2.Models
{
    public class PhoneC
    {
        [Key]
        public string cId { get; set; } = string.Empty;
        public string cPhone { get; set; } = string.Empty;
    }
}
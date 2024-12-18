using System.ComponentModel.DataAnnotations;

namespace CareerManagementApp.MODEL
{
    public class CareerSuggestion
    {
        [Key]
        public Guid ID { get; set; }
        public string Message { get; set; }

        public Guid UserID { get; set; }
        public User User { get; set; }

    }
}

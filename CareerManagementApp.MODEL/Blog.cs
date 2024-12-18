using System.ComponentModel.DataAnnotations;

namespace CareerManagementApp.MODEL
{
    public class Blog
    {
        [Key]
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
    }
}

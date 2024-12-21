using System.ComponentModel.DataAnnotations;

namespace CareerManagementApp.MODEL
{
    public class User
    {
        [Key]
        public Guid ID { get; set; }
        [EmailAddress]
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid RoleID { get; set; }

        public Role Role { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<AIChat> AIChats { get; set; }
        public List<CareerSuggestion> CareerSuggestions { get; set; }
    }
}
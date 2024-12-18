using System.ComponentModel.DataAnnotations;

namespace CareerManagementApp.MODEL
{
    public class Role
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }

        public List<User> Users { get; set; }
    }
}

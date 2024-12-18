using CareerManagementApp.MODEL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerManagementApp.DAL.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(a=>a.Role).WithMany(b=>b.Users).HasForeignKey(a=>a.RoleID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

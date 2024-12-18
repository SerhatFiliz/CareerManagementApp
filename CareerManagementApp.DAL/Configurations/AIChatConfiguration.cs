using CareerManagementApp.MODEL;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerManagementApp.DAL.Configurations
{
    public class AIChatConfiguration : IEntityTypeConfiguration<AIChat>
    {
        public void Configure(EntityTypeBuilder<AIChat> builder)
        {
            builder.HasOne(a => a.User).WithMany(b => b.AIChats).HasForeignKey(c => c.UserID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

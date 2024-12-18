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
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasOne(a=>a.User).WithMany(b=>b.Blogs).HasForeignKey(c=>c.UserID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

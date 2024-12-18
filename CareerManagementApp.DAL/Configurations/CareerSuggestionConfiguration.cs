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
    public class CareerSuggestionConfiguration : IEntityTypeConfiguration<CareerSuggestion>
    {
        public void Configure(EntityTypeBuilder<CareerSuggestion> builder)
        {
            builder.HasOne(a => a.User).WithMany(b => b.CareerSuggestions).HasForeignKey(c => c.UserID).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

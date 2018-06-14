using StudentManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem.Context
{
    public class DatabaseContext:DbContext
    {
        public DbSet<AssessmentDetails> AssessmentDetails { get; set; }
    }
}

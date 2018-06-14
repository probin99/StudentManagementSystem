using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem.Model
{
    public class AssessmentDetails : MarkedAssessment
    {
        public decimal AssessmentId { get; set; }
        public string Type { get; set; }
        public string Topic { get; set; }
        public string Format { get; set; }
        public string DueDate { get; set; }
    }
}

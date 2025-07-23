using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Entities
{
    public class DiamondGrades
    {
        public int LabAccountID { get; set; }
        public string LabAccountName { get; set; }
        public string Grade { get; set; }
        public string GradeDesc { get; set; }
        public int GradeOrder { get; set; }
    }
}

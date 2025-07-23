using DiamondTransaction.Domain.Entities;
using DiamondTransaction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Interfaces
{
    public interface IDiamondGradingRepository
    {
        Task<IEnumerable<DiamondSizeDto>> GetDiamondSizesAsync(string labname = null);
        Task<IEnumerable<DiamondGrades>> GetGradesByGradeTypeAndLabAsync(string gradeType, string labname = null);
    }
}

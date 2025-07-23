using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IDiamondGradingRepository
    {
        Task<IEnumerable<DiamondSizeDto>> GetDiamondSizesAsync(string labname = null);
        Task<IEnumerable<DiamondGrades>> GetGradesByGradeTypeAndLabAsync(string gradeType, string labname = null);
    }
}

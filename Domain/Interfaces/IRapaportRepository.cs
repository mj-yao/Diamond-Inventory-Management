using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.Domain.Interfaces
{
    public interface IRapaportRepository
    {
        List<string> GetRapaportShapeScale();
        List<string> GetRapaportGradingScale(string gradeType);

    }
}

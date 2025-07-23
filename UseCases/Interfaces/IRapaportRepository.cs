using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Interfaces
{
    public interface IRapaportRepository
    {
        List<string> GetRapaportShapeScale();
        List<string> GetRapaportGradingScale(string gradeType);
        string GetRapaportListPrice(string shape, string size, string color, string clarity);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiamondTransaction.UseCases.Interfaces;

namespace DiamondTransaction.UseCases.Services
{
    public class RapaportPriceService
    {
        private readonly IRapaportRepository _repository;
        public RapaportPriceService(IRapaportRepository repository)
        {
            _repository = repository;
        }
        public List<string> GetRapaportShapeScale()
        {
            return _repository.GetRapaportShapeScale();
        }
        public List<string> GetRapaportGradingScale(string gradeType)
        {
            return _repository.GetRapaportGradingScale(gradeType);
        }
         
        
        public bool IsValidRapShapeScale(string value)
        {
            var rapShapes = _repository.GetRapaportShapeScale();
            
            return rapShapes.Contains(value);
        }

        public bool IsValidRapGradingScale(string gradingType, string value)
        {
            var rapGradings = _repository.GetRapaportGradingScale(gradingType);
            return rapGradings.Contains(value);
        }

        public string GetRapaportListPrice(string shape, string size, string color, string clarity)
        {
            return _repository.GetRapaportListPrice(shape, size, color, clarity);
        }


    }
}

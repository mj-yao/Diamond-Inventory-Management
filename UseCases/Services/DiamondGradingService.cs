using DiamondTransaction.Domain.Entities;
using DiamondTransaction.UseCases.Interfaces;
using DiamondTransaction.UseCases.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Services
{
    public class DiamondGradingService
    {
        private readonly IDiamondGradingRepository _diamondGradingRepository;

        public DiamondGradingService(IDiamondGradingRepository diamondGradingRepository)
        {
            _diamondGradingRepository = diamondGradingRepository;
        }
        public async Task<IEnumerable<DiamondSizeDto>> GetDiamondSizesAsync(string labname = null)
        {
            return await _diamondGradingRepository.GetDiamondSizesAsync(labname);
        }


        public async Task<IEnumerable<DiamondGrades>> GetGradesByGradeTypeAndLabAsync(string gradeType, string labname = null)
        {
            return await _diamondGradingRepository.GetGradesByGradeTypeAndLabAsync(gradeType, labname);
        }

    }
}

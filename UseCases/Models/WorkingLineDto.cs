using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondTransaction.UseCases.Models
{
        public class WorkingLineDto
        {
            public DocLineDto DocLineDto { get; set; }
            public CertificateDto Certificate { get; set; }
            public SourceLotChangeDto SourceLotChangeDto { get; set; }

            public WorkingLineDto(DocLineDto docLine, CertificateDto certificate, SourceLotChangeDto sourceLotChange)
            {
                DocLineDto = docLine;
                Certificate = certificate;
                SourceLotChangeDto = sourceLotChange;
            }
        }
}

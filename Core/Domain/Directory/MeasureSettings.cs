using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Configuration;

namespace Core.Domain.Directory
{
    public class MeasureSettings : ISettings
    {
        public int BaseDimensionId { get; set; }
        public int BaseWeightId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationApp
{
    public partial class Services
    {
        public decimal TotalCost
        {
            get
            {
                return Tasks.Where(rec => rec.IdDifficulti == 1).Sum(t => t.Cost);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationApp
{
    public partial class AssignedTasks
    {
        public string VisibleEmplo
        {
            get
            {
                if (IdUser != null)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }
        public decimal CostTask
        {
            get
            {
                return Tasks.Cost / 2;
            }
        }
    }
}

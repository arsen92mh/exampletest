using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculationApp
{
    public partial class Users
    {
        public string VisibleCategory
        {
            get
            {
                if (IdRole == 2)
                    return "Visible";
                else
                    return "Collapsed";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{/// <summary>
/// this view model is used to create reports revenuse by activity and class
/// </summary>
    public class ActivityReport
    {


        public string  ActivityName { get; set; }

        public int NoOfBookings { get; set; }

        public double ActivityIncome { get; set; }

       


    }
}

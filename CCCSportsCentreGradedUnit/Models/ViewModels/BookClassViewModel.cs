using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{
    /// <summary>
    /// this view model is used to handle bookings
    /// </summary>
    public class BookClassViewModel
    {
        
        public Booking Booking { get; set; }
        public Member Member { get; set; }

        public double BookingTotal { get; set; }

        public ICollection <FitnessClass> FitnessClasses { get; set; }

        public ICollection<FitnessActivity> FitnessActivities { get; set; }



    }
}

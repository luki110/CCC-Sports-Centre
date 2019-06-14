using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{
    /// <summary>
    /// this view model is used to display details of booking
    /// </summary>
    public class BookingDetailsViewModel
    {
        /// <summary>
        /// booking
        /// </summary>
        public Booking Booking { get; set; }

        /// <summary>
        /// list of fintes classes
        /// </summary>
        public List<FitnessClass> FitnessClasses { get; set; }
        /// <summary>
        /// lsit of fitness activities
        /// </summary>
        public List<FitnessActivity> FitnessActivities { get; set; }

    }
}

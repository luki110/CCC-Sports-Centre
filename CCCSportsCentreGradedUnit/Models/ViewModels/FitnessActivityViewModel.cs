using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{
    /// <summary>
    /// this view model is used to create new fitness activity
    /// </summary>
    public class FitnessActivityViewModel
    {
        /// <summary>
        /// fitnessa activity 
        /// </summary>
        public FitnessActivity FitnessActivity { get; set; }
        /// <summary>
        /// list of rooms
        /// </summary>
        public IEnumerable<Room> Rooms { get; set; }
        /// <summary>
        /// list of FitnessActivityCategories
        /// </summary>
        public IEnumerable<FitnessActivityCategory> FitnessActivityCategories { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models.ViewModels
{   
    /// <summary>
    /// this view model is used to create new fitness activity
    /// </summary>
    public class FitnessClassViewModel
    {
        /// <summary>
        /// fitness class
        /// </summary>
        public FitnessClass FitnessClass { get; set; }

        /// <summary>
        /// list of rooms
        /// </summary>
        public IEnumerable<Room> Rooms { get; set; }

        /// <summary>
        /// list of FitnessClassCategories
        /// </summary>
        public IEnumerable<FitnessClassCategory> FitnessClassCategories { get; set; }
    }
}

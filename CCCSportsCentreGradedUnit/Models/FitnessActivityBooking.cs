using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// this class handles many to many relationship between FitnessActivity and Booking
    /// </summary>
    public class FitnessActivityBooking
    {
        /// <summary>
        /// stores Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Stores FitnessActivityId
        /// </summary>
        [Display(Name = "Fitness Activity")]
        public int FitnessActivityId { get; set; }
        /// <summary>
        /// Links FitnessActivity to FitnessActivityId as foreign key
        /// </summary>
        [ForeignKey("FitnessActivityId")]
        public virtual FitnessActivity FitnessActivity { get; set; }

        /// <summary>
        /// stores bookingId
        /// </summary>
        [Display(Name = "Booking")]
        public int BookingId { get; set; }
        /// <summary>
        /// Links Booking to BookingId as foreign key
        /// </summary>
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }




    }
}

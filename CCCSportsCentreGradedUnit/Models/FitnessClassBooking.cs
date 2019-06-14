using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// This class handles many to many relationship between booking table and fitnessClass table
    /// </summary>
    public class FitnessClassBooking
    {
        /// <summary>
        /// stores Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// stores fitnessClass Id
        /// </summary>
        [Display(Name = "Fitness Class")]
        public int FitnessClassId { get; set; }

        /// <summary>
        /// Links FitnessClass to FitnessClassId as foreign key
        /// </summary>
        [ForeignKey("FitnessClassId")]
        public virtual FitnessClass FitnessClass { get; set; }
        /// <summary>
        /// stores bookingId
        /// </summary>
        [Display(Name = "Booking")]
        public int BookingId { get; set; }
        /// <summary>
        /// Links Booking to bookingId as foreign key
        /// </summary>
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    public class Booking
    {
        /// <summary>
        /// stores Id
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Stores when booking was made
        /// </summary>
        public DateTime BookingDate { get; set; }
        /// <summary>
        /// stores total price of booking
        /// </summary>
        public double BookingTotal { get; set; }

        /// <summary>
        /// stores memberId
        /// </summary>
        [Display(Name = "Member")]
        public string MemberId { get; set; }
        /// <summary>
        /// Link Member to MemberId as foreign key
        /// </summary>
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        /// <summary>
        /// stores payment id -- this is needed for refunds
        /// </summary>
        public string BookingPaymentId { get; set; }
        /// <summary>
        /// stores when payment was made
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// stores if payment is confirmed
        /// </summary>
        public bool IsPaymentConfirmed { get; set; }

        /*public Booking()
        {
            FitnessClasses = new List<FitnessClass>();
            FitnessActivities = new List<FitnessActivity>();
        }
        // i tried to make many to many relationship but entity framweork was throwing an error so i created 2 new classes
        public virtual ICollection<FitnessClass> FitnessClasses { get; set; }

        public virtual ICollection<FitnessActivity> FitnessActivities { get; set; }*/


    }
}

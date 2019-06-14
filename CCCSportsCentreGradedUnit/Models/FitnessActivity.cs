using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    public class FitnessActivity
    {
        /// <summary>
        /// stores Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// stores start date
        /// </summary>
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name ="Start Date")]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// stores start time
        /// </summary>
        [Required]
        [Display(Name = "Start Time")]        
        public DateTime StartTime { get; set; }
        /// <summary>
        /// stores end time
        /// </summary>
        [Required]
        [Display(Name = "End Date")]        
        public DateTime EndTime { get; set; }
        /// <summary>
        /// stores duration
        /// </summary>
        [Required]
        [Display(Name = "Duration (in minutes)")]
        public int Duration { get; set; }

        /// <summary>
        /// stores weter this activity is available
        /// </summary>
        public bool Available { get; set; }
        /// <summary>
        /// stores price
        /// </summary>
        [Required]        
        public double Price { get; set; }

        /*public FitnessActivity()
        {
            Bookings = new List<Booking>();
        }*/
        /// <summary>
        /// stores roomsId
        /// </summary>
        [Display(Name = "Room")]
        public int RoomId { get; set; }
        /// <summary>
        /// Links Room to RoomId as foreign key
        /// </summary>
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
        /// <summary>
        /// stores FitnessActivityCategoryId
        /// </summary>
        [Display(Name ="Activity Category")]
        public int FitnessActivityCategoryId { get; set; }

        /// <summary>
        /// Links FitnessActivityCategory to FitnessActivityCategoryId as foreign key
        /// </summary>
        [ForeignKey("FitnessActivityCategoryId")]
        public virtual FitnessActivityCategory FitnessActivityCategory { get; set; }

        //public virtual ICollection<Booking> Bookings { get; set; }



    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    public class FitnessClass
    {
        /// <summary>
        /// stores id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// stores start date
        /// </summary>
        [Required]
        [Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
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
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// stores duration
        /// </summary>
        [Required]
        [Display(Name = "Duration (in minutes)")]
        [DataType(DataType.Duration)]
        public int Duration { get; set; }
        /// <summary>
        /// stores price
        /// </summary>
        [Required]
        public double Price { get; set; }

        /// <summary>
        /// stores number of people who booked class
        /// </summary>
        public int NoOfPeopleBooked { get; set; }

        /// <summary>
        /// stores weter this class is available
        /// </summary>
        public bool Available { get; set; }
        /*public FitnessClass()
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
        /// stores fitnessClassCategoryId
        /// </summary>
        [Display(Name = "Fitness Class Category")]
        public int FitnessClassCategoryId { get; set; }

        /// <summary>
        /// Links FitnessClassCategory to FitnessClassCategoryId as foreign key
        /// </summary>
        [ForeignKey("FitnessClassCategoryId")]
        public virtual FitnessClassCategory FitnessClassCategory { get; set; }

        //public virtual ICollection<Booking> Bookings { get; set; }



    }
}

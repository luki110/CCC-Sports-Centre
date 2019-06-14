using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CCCSportsCentreGradedUnit.Models
{
    /// <summary>
    /// Room class
    /// </summary>
    public class Room
    {
        /// <summary>
        /// stores room id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// stores how many people can fit into room
        /// </summary>
        [Required]
        public int Capacity { get; set; }

        /// <summary>
        /// stores room's "name"
        /// </summary>
        [Required]
        public string Name { get; set; }


    }
}

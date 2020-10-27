using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonoVehicle.Models
{
    public class VehicleMake
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Make Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Abreviation must be 3 characters long")]
        [Display(Name = "Make Abbreviation")]
        public string Abrv { get; set; }


        public ICollection<VehicleModel> VehicleModels { get; set; }
    }
}

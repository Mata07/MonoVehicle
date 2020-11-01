﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MonoVehicle.Models.ViewModels
{
    public class VehicleModelViewModel
    {        
        public int Id { get; set; }
        public int MakeId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Model Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Model Abbreviation")]
        public string Abrv { get; set; }        

        //public VehicleMake Make { get; set; }

        [Display(Name = "Make Name")]
        public string MakeName { get; set; }
    }
}

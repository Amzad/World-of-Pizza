﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Topping
    {
        [Key]
        public int ToppingID { get; set; }
        public string ToppingName { get; set; }
        public decimal ToppingPrice { get; set; }
        public string ToppingType { get; set; }

        public int PizzaID { get; set; }
        public virtual Pizza Pizza { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Revature_Project2API.Models
{
    public class Pizza
    {
        [Key]
        public int PizzaID { get; set; }
        public string PizzaType { get; set; }
        public string PizzaSauce { get; set; }
        public string PizzaBread { get; set; }

        public int OrderDetailID { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual ICollection<Topping> Toppings { get; set; }
        
        
    }
}

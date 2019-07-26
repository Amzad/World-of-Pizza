﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Revature_Project2API.Models
{
    public class PizzaDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        //this table is extra. 
        public virtual int OrderID { get; set; }
        public decimal OrderDetailPrice { get; set; }
        public virtual Order Order { get; set; }

        public virtual ICollection<Pizza> Pizzas { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NJCCEvents.Models
{
    public class Aim
    {
        [Key]
        public int Id { get; set; }
        public string AimType { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Database.Context.DataContracts.Entities
{
    public class Owner
    {
        [Key]
        public int Id { get; set; }
    }
}

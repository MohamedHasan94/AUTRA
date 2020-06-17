using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AUTRA.Models
{
    [Table("Project")]
    public class Project
    {
        [Key]
        [Column(Order = 1)]
        public string Fk_UserId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string Name { get; set; }

        [ForeignKey("Fk_UserId")]
        public ApplicationUser User { get; set; }
    }
}

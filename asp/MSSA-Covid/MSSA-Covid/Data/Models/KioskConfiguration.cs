using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MSSA_Covid.Data.Models
{
    public class KioskConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public List<IFormFile>RawFiles { get; set; }


        public virtual IList<KioskFile> KioskFiles { get; set; }
    }
}

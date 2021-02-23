using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MSSA_Covid.Data.Interfaces;

namespace MSSA_Covid.Data.Models
{
    public class KioskFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BlobUri { get; set; }

        [NotMapped]
        public IFormFile RawFile { get; set; }

        //Relationships
        public virtual int KioskConfigurationId { get; set; }
        public virtual KioskConfiguration KioskConfiguration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MSSA_Covid.Data.Models
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string County { get; set; }        
        public string ImageName { get; set; }
        [Display(Name = "QR Code")]
        public string ImageBlobUrl { get; set; }
        [Display(Name = "COVID URL")]
        public Uri Url { get; set; }
        [NotMapped]
        public string Name { get
            {
                return String.Format("{0}, {1}", City, State);
            }
        }
        [NotMapped]
        public IFormFile QRCodeImage { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIDemo.Model
{
    public class ApplicationUser:IdentityUser
    {
        [Column(TypeName="nvarchar(150)")]
        public String FullName { get; set; }
        
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models
{
    public class Profile
    {

        [Key]
        public int ProfileId { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models
{
    /// <summary>
    /// Represents a single profile
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// The unique identifier for each Profile
        /// </summary>
        [Key]
        public int ProfileId { get; set; }

        /// <summary>
        /// The name of the profile
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The logged in user's id
        /// </summary>
        public string UserId { get; set; }
    }

    public class ProfileDisplayTaskViewModel
    {
        public int ProfileId { get; set; }

        public string Name { get; set; }

        public List<Task>? AllTasks { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace To_Do_List.Models
{
    /// <summary>
    /// Represents a single task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// The unique identifier for each Task
        /// </summary>
        [Key]
        public int TaskId { get; set; }

        /// <summary>
        /// The title of the task
        /// </summary>
        [Required]
        [StringLength(75)]
        public string Title { get; set; }

        /// <summary>
        /// The description of the task
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// The person assigned to complete the task
        /// </summary>
        public Profile Assignee { get; set; }
    }

    public class TaskCreateViewModel 
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public List<Profile>? AllProfiles { get; set; }

        public int ChosenProfile { get; set; }
    }
}

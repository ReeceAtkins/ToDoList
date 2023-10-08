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
        public string Assignee { get; set; }
    }
}

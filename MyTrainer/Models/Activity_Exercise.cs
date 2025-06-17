using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTrainer.Models
{
    public class Activity_Exercise
    {
        [Key]
        public int Id { get; set; }
        public int activityId { get; set; }

        [ForeignKey("activityId")]
        public Activity_history activity_History { get; set; }
        public int exerciseId { get; set; }

        [ForeignKey("exerciseId")]
        public Exercise Exercise { get; set; }
        public int sets { get; set; }
        public int reps {  get; set; } 
        public double weightUsed { get; set; }
        public double calBurned { get; set; }
    }
}

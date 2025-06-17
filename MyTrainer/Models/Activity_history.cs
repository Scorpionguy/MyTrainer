using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTrainer.Models
{
    public class Activity_history
    {
        [Key]
        public int id { get; set; }
        public string userId { get; set; }

        [ForeignKey("userId")]
        public User User { get; set; }
        public List<Activity_Exercise> activityExercise { get; set; } = new();
        public DateTime startDateTime{ get; set; }
        public DateTime endDateTime { get; set; }
        public double? totalCal { get; set; }
        public string coment { get; set; }
        public bool finished { get; set; } = false;
    }
}

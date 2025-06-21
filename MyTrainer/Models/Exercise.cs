using System.ComponentModel.DataAnnotations;

namespace MyTrainer.Models
{
    public class Exercise
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string howToUrl { get; set; }
        public double standatdCal { get; set; }
        public string description { get; set; }
        public string targetMuscle { get; set; }
        public bool confirmed { get; set; }
        public List<Activity_Exercise> activity_Exercises { get; set; } = new();
    }
}

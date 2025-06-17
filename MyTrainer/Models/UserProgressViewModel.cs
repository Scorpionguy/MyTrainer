using System.ComponentModel.DataAnnotations;

namespace MyTrainer.Models
{
    public class UserProgressViewModel
    {
        public double startWeight { get; set; }
        public double goalWeight { get; set; }
        public double CurrentWeight { get; set; }
        public List<Weight_history> Weights { get; set; }
        public newWeight add {  get; set; }
    }

    public class newWeight
    {
        [Required]
        [Range(30, 300, ErrorMessage = "Введите вес от 30 до 300 кг")]
        public double Weight { get; set; }
    }
}

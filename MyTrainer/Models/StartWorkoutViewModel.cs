namespace MyTrainer.Models
{
    public class StartWorkoutViewModel
    {
        public int SelectedExerciseId { get; set; }
        public List<ExerciseSetViewModel> Sets { get; set; } = new();
        public List<ExerciseInfo> AvailableExercises { get; set; } = new();

    }
    public class ExerciseSetViewModel
    {
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
    public class ExerciseInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? HowToUrl { get; set; }
        public double? calBurned { get; set; }
    }
    public class ExerciseSetInputModel
    {
        public int Reps { get; set; }
        public double Weight { get; set; }
        public double calBurned { get;set; }
    }

    public class ExerciseInWorkoutInputModel
    {
        public int ExerciseId { get; set; }
        public List<ExerciseSetInputModel> Sets { get; set; } = new();
    }

    public class StartWorkoutInputModel
    {
        public List<ExerciseInWorkoutInputModel> Exercises { get; set; } = new();
        public DateTime StartTime { get; set; }
        public double burnedCals { get; set; }
    }
}

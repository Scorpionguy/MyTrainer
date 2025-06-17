namespace MyTrainer.Models

{
    public class AllWorkoutsModel
    {
        public List<WorkoutsModel> Workouts { get; set; }
        public int CurrentWorkoutIndex { get; set; }
    }
    public class WorkoutsModel
    {
        public Activity_history? LatestWorkout { get; set; }
        public List<Activity_ExerciseViewModel> ExercisesOfLatestWorkout { get; set; } = new();
    }
    public class Activity_ExerciseViewModel
    {
        public string ExerciseName { get; set; } = "";
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double WeightUsed { get; set; }
        public double CalBurned { get; set; }
    }
    
}

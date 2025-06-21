namespace MyTrainer.Models
{
    public class AdminDashboardViewModel
    {
        public List<User> Users { get; set; }
        public List<Exercise> Exercises { get; set; }
        public List<Exercise> NotConfirmed { get; set; }
    }
}

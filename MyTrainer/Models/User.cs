using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyTrainer.Models
{
    public class User : IdentityUser
    {
        
        public string name { get; set; }
        public string lastname { get; set; }
        public double startWeight { get; set; }
        public double goalWeight {  get; set; }
        public List<Weight_history> weightHistory { get; set; } = new();
        public List<Activity_history> activityHistory { get; set; } = new();
		public DateTime birthDay { get; set; }
		public List<UserPost> postHistory { get; set; }
    }
}

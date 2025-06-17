using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTrainer.Models
{
    public class Weight_history
    {
        [Key]
        public int id { get; set; }
        public string userId { get; set; }

        [ForeignKey("userId")]
        public User User { get; set; }
        public double weight { get; set; }
        public DateTime updateTime { get; set; }
    }
}

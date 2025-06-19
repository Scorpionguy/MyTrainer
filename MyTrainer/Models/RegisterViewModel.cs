using System.ComponentModel.DataAnnotations;

namespace MyTrainer.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage ="Неверный формат почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage ="Пароль должен содержать минимум 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        public string Lastname { get; set; }

		[Required(ErrorMessage = "Введите дату рождения")]
		[DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

		[Required(ErrorMessage = "Введите номер телефона")]
		[Phone]
        public string? PhoneNumber { get; set; }
    }
}

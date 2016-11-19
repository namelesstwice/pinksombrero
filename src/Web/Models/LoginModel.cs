using System.ComponentModel.DataAnnotations;

namespace PinkSombrero.Web
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Имя обязательно к заполнению")]
		[RegularExpression(@".+?[/\\@].+", ErrorMessage = "Нужно ввести доменное имя в формате ALS\\i.ivanov")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Пароль обязателен к заполнению")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
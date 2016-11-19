using System.ComponentModel.DataAnnotations;

namespace PinkSombrero.Web
{
	public class CreateMongoCredentialsModel
	{
		public string Login { get; set; }

		[Required(ErrorMessage = "Пароль обязателен к заполнению")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
namespace PinkSombrero.Core
{
	/// <summary>
	/// Права доступа к сервису
	/// </summary>
	/// <remarks>
	/// Каждый последующий уровень включает в себя предыдущий
	/// </remarks>
	public enum AccessRights
	{
		/// <summary>
		/// Не может пользоваться
		/// </summary>
		None = 0,

		/// <summary>
		/// Может получить перманентную учётку в MongoDB на чтение
		/// </summary>
		Read = 1,

		/// <summary>
		/// Может получить временную учётку в MongoDB на запись
		/// </summary>
		ReadAndWrite = 2,

		/// <summary>
		/// Может управлять другими пользователями
		/// </summary>
		Admin = 3
	}
}
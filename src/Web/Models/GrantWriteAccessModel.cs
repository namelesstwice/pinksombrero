using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PinkSombrero.Core;

namespace PinkSombrero.Web
{
	public class GrantWriteAccessModel
	{
		private GrantWriteAccessModel()
		{
		}

		public GrantWriteAccessModel(User user, SelectListItem[] availableClusterIds)
		{
			AvailableClusterIds = availableClusterIds;
			if (user.WriteSession != null)
				WriteExpiration = user.WriteSession.ExpireTime - DateTime.UtcNow;
		}

		public TimeSpan? WriteExpiration { get; }

		public SelectListItem[] AvailableClusterIds { get; private set; }

		[Required(ErrorMessage = "Поле обязательно")]
		public string Reason { get; set; }

		[Required(ErrorMessage = "Поле обязательно")]
		public Period WriteModePeriod { get; set; }

		[Required(ErrorMessage = "Поле обязательно")]
		public string[] ClusterIds { get; set; }
	}

	public enum Period
	{
		[Display(Name = "1 минута")]
		Minute,

		[Display(Name = "5 минут")]
		FiveMinutes,

		[Display(Name = "15 минут")]
		FifteenMinutes,

		[Display(Name = "30 минут")]
		HalfAnHour
	}
}
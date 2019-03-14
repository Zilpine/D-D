using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DeDCalculator.Data.DAL
{
	public class ApplicationUser:IdentityUser<Guid>, IBaseEntity
	{
		#region BaseEntity Implementation

		public virtual bool IsValid { get; set; }
		public virtual DateTime CreatedAt { get; set; }
		public virtual DateTime UpdatedAt { get; set; }

		#endregion

	}

}

using Microsoft.AspNetCore.Identity;
using System;

namespace DeDCalculator.Data.DAL
{
	public class ApplicationRole : IdentityRole<Guid>
	{
		public ApplicationRole()
		{
		}

		public ApplicationRole(string roleName) : base(roleName)
		{
		}
	}
}

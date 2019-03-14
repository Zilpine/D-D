using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeDCalculator.Data.DAL
{
	public interface IEntity
	{
		Guid Id { get; set; }
	}

	public interface IBaseEntity : IEntity
	{
		bool IsValid { get; set; }
		DateTime CreatedAt { get; set; }
		DateTime UpdatedAt { get; set; }
	}

	public abstract class BaseEntity : IBaseEntity
	{
		[Key] public virtual Guid Id { get; set; }

		public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public virtual bool IsValid { get; set; } = true;

		protected BaseEntity()
		{
			Id = Guid.NewGuid();
		}
	}
}

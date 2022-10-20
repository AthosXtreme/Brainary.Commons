using System.ComponentModel.DataAnnotations;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Basic abstraction for entities
    /// </summary>
    public abstract class Entity
	{
		public abstract object? GetId();
	}

    /// <summary>
    /// Typed implementation of <see cref="Entity"/>
    /// </summary>
    public abstract class Entity<T> : Entity
	{
		[Key]
		[Display(Order = 0)]
		public T? Id { get; set; }

		public override object? GetId()
		{
			return Id;
		}
	} 
}

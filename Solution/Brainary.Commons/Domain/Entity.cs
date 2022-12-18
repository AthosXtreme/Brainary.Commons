using System.ComponentModel.DataAnnotations;

namespace Brainary.Commons.Domain
{
    /// <summary>
    /// Basic abstraction for entities
    /// </summary>
    public abstract class Entity
    {
        protected object? id;

        public object? Id { get => id; set => id = value; }
    }

    /// <summary>
    /// Typed implementation of <see cref="Entity"/>
    /// </summary>
    public abstract class Entity<T> : Entity
	{
		[Key]
		[Display(Order = 0)]
		[MaxLength(16)] //used when text type
		public new T? Id { get => id != null ? (T?)id : default; set => id = value; }
	} 
}

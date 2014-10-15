namespace Brainary.Commons.Domain
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Basic entity interface with int Id
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }
    }


    /// <summary>
    /// Implementation of <see cref="IEntity"/>
    /// </summary>
    public abstract class Entity : IEntity
    {
        [Display(Order = 0)]
        public int Id { get; set; }
    }
}
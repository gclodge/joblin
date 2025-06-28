namespace Joblin.Domain.Abstractions;

/// <summary>
/// Represents an entity that has a unique identifier.
/// </summary>
public interface IHasId<T>
    where T : class, IEquatable<T>
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public T Id { get; }
}
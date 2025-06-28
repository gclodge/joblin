namespace Joblin.Application.Common.Abstractions;

public interface ICacheService
{
    /// <summary>
    /// Retrieves an item from the cache by its key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets an item in the cache with an optional absolute expiration time.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="absoluteExpiration"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, CancellationToken cancellationToken = default) where T : class;
    
    /// <summary>
    /// Removes an item from the cache by its key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all items from the cache that start with the specified prefix key.
    /// </summary>
    /// <param name="prefixKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}

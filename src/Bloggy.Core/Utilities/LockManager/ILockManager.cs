namespace Bloggy.Core.Utilities.LockManager;

public interface ILockManager
{
    /// <summary>
    /// Check a string key whether it is locked
    /// </summary>
    /// <param name="key">Locking key</param>
    /// <returns>True if the key is already locked and false if it isn't locked..</returns>
    Task<bool> IsLockedAsync(string key);

    /// <summary>
    /// Lock a string key (retention policy = 1 day)
    /// </summary>
    /// <param name="key">The operation name, for example: filling.</param>
    /// <param name="duration">Locking lifetime</param>
    /// <returns>True if the operation is previously locked and false if it has locked now.</returns>
    Task<LockResult> LockAsync(string key);

    /// <summary>
    /// Lock a string key
    /// </summary>
    /// <param name="key">The operation name, for example: filling.</param>
    /// <param name="ttl">Locking lifetime</param>
    /// <param name="isSliding">Set sliding time to live is lock is existing</param>
    /// <returns>True if the operation is previously locked and false if it has locked now.</returns>
    Task<LockResult> LockByTtlAsync(string key, TimeSpan ttl, bool isSliding = false);

    /// <summary>
    /// Remove locked key
    /// </summary>
    /// <param name="key">The operation name, for example: filling.</param>
    /// <returns>True if key successfully removed</returns>
    public Task<bool> UnlockAsync(string key);
}
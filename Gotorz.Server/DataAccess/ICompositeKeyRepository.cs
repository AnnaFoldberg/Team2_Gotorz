namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// A repository interface for managing entities with composite keys.
    /// Extends the functionality of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public interface ICompositeKeyRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves a specific entity of type <typeparamref name="T"/> by its composite key.
        /// </summary>
        /// <param name="key1">The first part of the composite key of the entity to retrieve.</param>
        /// <param name="key2">The second part of the composite key of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/> that matches the specified composite key.</returns>
        T GetByCompositeKey(int key1, int key2);

        /// <summary>
        /// Retrieves all entities that are associated with the specified <paramref name="key1"/>.
        /// </summary>
        /// <param name="key1">The first part of the composite key of the entities to retrieve.</param>
        /// <returns>A collection of entities of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetByKeyOne(int key1);

        /// <summary>
        /// Retrieves all entities that are associated with the specified <paramref name="key2"/>.
        /// </summary>
        /// <param name="key2">The second part of the composite key of the entities to retrieve.</param>
        /// <returns>A collection of entities of type <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetByKeyTwo(int key2);

        /// <summary>
        /// Deletes an entity from the database, identified by its composite key.
        /// </summary>
        /// <param name="key1">The first part of the composite key of the entity to delete.</param>
        /// <param name="key2">The second part of the composite key of the entity to delete.</param>
        void Delete(int key1, int key2);
    }
}

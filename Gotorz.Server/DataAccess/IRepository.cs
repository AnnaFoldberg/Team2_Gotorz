namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// Generic interface for repositories. Provides CRUD (Create, Read, Update, and Delete) operations.
    /// </summary>
    /// <typeparam name="T">The entity type managed by the repository.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <returns>A collection of entities of type <typeparamref name="T"/>.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="T"/> by its primary key.
        /// </summary>
        /// <param name="key">The primary key of the entity to retrieve.</param>
        /// <returns>The entity of type <typeparamref name="T"/> that matches the specified <paramref name="key"/>.</returns>
        Task<T?> GetByKeyAsync(int key);

        /// <summary>
        /// Adds a new entity of type <typeparamref name="T"/> to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type <typeparamref name="T"/> in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity of type <typeparamref name="T"/>, identified by its primary key.
        /// </summary>
        /// <param name="key">The primary key of the entity to delete.</param>
        Task DeleteAsync(int key);
    }
}
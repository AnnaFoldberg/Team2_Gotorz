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
        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adds a new entity of type <typeparamref name="T"/> to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        Task<T> AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type <typeparamref name="T"/> in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        Task UpdateAsync(T entity);
    }
}
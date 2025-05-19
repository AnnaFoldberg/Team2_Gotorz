using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// Interface for performing CRUD operations on Hotel entities.
    /// </summary>
    /// <author>Sayeh</author>
    public interface IHotelRepository
    {
        /// <summary>
        /// Retrieves all hotels from the database.
        /// </summary>
        Task<IEnumerable<Hotel>> GetAllAsync();

        /// <summary>
        /// Retrieves a single hotel by its ID.
        /// </summary>
        Task<Hotel?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new hotel to the database.
        /// </summary>
        Task AddAsync(Hotel hotel);

        /// <summary>
        /// Updates an existing hotel in the database.
        /// </summary>
        Task UpdateAsync(Hotel hotel);

        /// <summary>
        /// Deletes a hotel from the database by ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// An interface for managing <see cref="HotelRoom"/> entities.
    /// </summary>
    /// <remarks>Provides method signatures for accessing and modifying hotel room records.</remarks>
    /// <author>Sayeh</author>
    public interface IHotelRoomRepository
    {
        /// <summary>
        /// Retrieves all <see cref="HotelRoom"/> entries from the database.
        /// </summary>
        Task<IEnumerable<HotelRoom>> GetAllAsync();

        /// <summary>
        /// Retrieves a single <see cref="HotelRoom"/> by its primary key.
        /// </summary>
        Task<HotelRoom?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all rooms matching the given <c>ExternalHotelId</c>.
        /// </summary>
        Task<IEnumerable<HotelRoom>> GetByHotelIdAsync(int hotelId);

        /// <summary>
        /// Adds a new <see cref="HotelRoom"/> to the database.
        /// </summary>
        Task AddAsync(HotelRoom room);

        /// <summary>
        /// Updates an existing <see cref="HotelRoom"/> in the database.
        /// </summary>
        Task UpdateAsync(HotelRoom room);

        /// <summary>
        /// Deletes a <see cref="HotelRoom"/> entry from the database by ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
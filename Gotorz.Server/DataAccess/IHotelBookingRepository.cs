using Gotorz.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// An interface for managing <see cref="HotelBooking"/> entities.
    /// </summary>
    /// <remarks>Defines method signatures for interacting with hotel booking records.</remarks>
    /// <author>Sayeh</author>
    public interface IHotelBookingRepository
    {
        /// <summary>
        /// Retrieves all <see cref="HotelBooking"/> records from the database.
        /// </summary>
        Task<IEnumerable<HotelBooking>> GetAllAsync();

        /// <summary>
        /// Retrieves a specific <see cref="HotelBooking"/> by its ID.
        /// </summary>
        Task<HotelBooking?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new <see cref="HotelBooking"/> record to the database.
        /// </summary>
        Task AddAsync(HotelBooking booking);

        /// <summary>
        /// Updates an existing <see cref="HotelBooking"/> record.
        /// </summary>
        Task UpdateAsync(HotelBooking booking);

        /// <summary>
        /// Deletes a <see cref="HotelBooking"/> from the database by its ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
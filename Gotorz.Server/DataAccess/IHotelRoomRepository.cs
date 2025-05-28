using Gotorz.Server.Models;

namespace Gotorz.Server.DataAccess
{
    /// <summary>
    /// An interface for managing <see cref="HotelRoom"/> entities.
    /// </summary>
    /// <remarks>Provides method signatures for accessing and modifying hotel room records.</remarks>
    /// <author>Sayeh</author>
    public interface IHotelRoomRepository : IRepository<HotelRoom>
    {
        /// <summary>
        /// Retrieves all rooms matching the given <c>ExternalHotelId</c>.
        /// </summary>
        Task<IEnumerable<HotelRoom>> GetByHotelIdAsync(int hotelId);
    }
}
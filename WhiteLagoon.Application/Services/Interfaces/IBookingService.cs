using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.Interfaces
{
    public interface IBookingService
    {
        Booking FinalizeBookingByUser(int villaId, DateOnly checkInDate, int nights, string userId);
        void FinalizeBooking(Booking booking);
        void BookingConfirmation(int bookingId, string sessionId, string sessionPaymentIntentId);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        Booking BookingDetails(int bookingId);
        void CheckIn(Booking bookingDetail);
        void CheckOut(Booking bookingDetail);
        void CancelBooking(Booking bookingDetail);
        Booking GetBookingById(int bookingId);
        List<Booking> GetAllByStatus();
        List<Booking> GetAll();
        List<int> AssignAvailableVillaNumberByVilla(int villaId, DateOnly checkInDate);
    }
}

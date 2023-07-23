using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.D.Interfaces
{
    public interface IBookingService
    {
        Booking FinalizeBookingByUser(int villaId, DateOnly checkInDate, int nights, string userId);
        void FinalizeBooking(Booking booking);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        void BookingConfirmation(int bookingId, string sessionId, string sessionPaymentIntentId);
        Booking BookingDetails(int bookingId);
        void CancelBooking(Booking booking);
        void CheckIn(Booking booking);
        void CheckOut(Booking booking);
        List<Booking> GetAllByStatus();
        Booking GetBookingById(int bookingId);
        List<int> AssignAvailableVillaNumberByVilla(int villaId, DateOnly checkInDate);
        List<Booking> GetAll();
        WordDocument GeneratePDF(int id, string basePath);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking entity);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        void UpdateStatus(int bookingId, string orderStatus, int villaNumber);
        //public Booking FinalizeBookingByUser(int villaId, DateOnly checkInDate, int nights, ApplicationUser user);
        //public void FinalizeBooking(Booking booking, double villaPrice);

    }
}

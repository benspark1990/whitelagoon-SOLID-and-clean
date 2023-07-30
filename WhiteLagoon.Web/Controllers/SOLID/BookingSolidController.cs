using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Application.Services.SOLID.D.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers.SOLID
{
    public class BookingSolidController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> _bookedStatus = new List<string> { "Approved", "CheckedIn" };
        private readonly Application.Services.SOLID.D.Interfaces.IBookingService _bookingCorrectSolidService;
        private readonly IVillaService _villaService;
        private readonly IVillaNumberService _villaNumberService;
        public BookingSolidController(
            IWebHostEnvironment webHostEnvironment,
            Application.Services.SOLID.D.Interfaces.IBookingService bookingCorrectSolidService,
            IVillaService villaService,
            IVillaNumberService villaNumberService)
        {
            _webHostEnvironment = webHostEnvironment;
            _bookingCorrectSolidService = bookingCorrectSolidService;
            _villaService = villaService;
            _villaNumberService = villaNumberService;
        }
        [Authorize]
        //INSTALL JSDELIVER add client side and search for jquery-ajax-unobtrusive@3.2.6
        //show if only admin were allowed then this will not work and go to access denided
        //[Authorize(Roles =SD.Role_Admin)]
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var booking = _bookingCorrectSolidService.FinalizeBookingByUser(villaId, checkInDate, nights, userId);

            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            _bookingCorrectSolidService.FinalizeBooking(booking);
            var villa = _villaService.GetById(booking.VillaId);

            //it is a regular customer account and we need to capture payment
            //stripe logic
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/finalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100), // $20.50 => 2050
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        //Images = new List<string>()
                        //        {
                        //            Request.Scheme + "://" + Request.Host.Value + villa.ImageUrl.Replace('\\','/')
                        //        },

                    }

                },
                Quantity = 1
            });


            var villaNumbersList = _villaNumberService.GetAll();
            var bookedVillas = _bookingCorrectSolidService.GetAllByStatus();

            int roomsAvailable = SD.VillaRoomsAvailable_Count(villa, villaNumbersList,
                booking.CheckInDate, booking.Nights, bookedVillas);
            if (roomsAvailable == 0)
            {
                TempData["error"] = "Room has been sold out!";
                //no rooms available
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }

            var service = new SessionService();

            Session session = service.Create(options);
            _bookingCorrectSolidService.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            var booking = _bookingCorrectSolidService.GetBookingById(bookingId);
            if (booking.Status == SD.StatusPending)
            {
                //this is a pending order
                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _bookingCorrectSolidService.BookingConfirmation(bookingId, session.Id, session.PaymentIntentId);
                }
            }

            return View(bookingId);
        }


        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            var bookingFromDb = _bookingCorrectSolidService.BookingDetails(bookingId);
            return View(bookingFromDb);
        }

        [HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _bookingCorrectSolidService.CheckIn(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _bookingCorrectSolidService.CheckOut(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingCorrectSolidService.CancelBooking(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult GeneratePDF(int id)
        {
            var doc = _bookingCorrectSolidService.GeneratePDF(id, _webHostEnvironment.WebRootPath);

            using DocIORenderer render = new();
            //Converts Word document into PDF document
            PdfDocument pdfDocument = render.ConvertToPDF(doc);

            //Saves the PDF document to MemoryStream.
            MemoryStream stream = new();
            pdfDocument.Save(stream);
            stream.Position = 0;

            //Download PDF document in the browser.
            return File(stream, "application/pdf", "BookingDetails.pdf");
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll(string status = "")
        {
            IEnumerable<Booking> objBookings;


            if (User.IsInRole(SD.Role_Admin))
            {
                objBookings = _bookingCorrectSolidService.GetAll();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _bookingCorrectSolidService.GetAll();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                objBookings = objBookings.Where(u => u.Status?.ToLower() == status.ToLower());
            }

            return Json(new { data = objBookings });
        }

        #endregion
    }
}

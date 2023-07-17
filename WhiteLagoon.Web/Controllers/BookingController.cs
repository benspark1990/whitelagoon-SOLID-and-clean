﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly List<string> _bookedStatus = new List<string> { "Approved", "CheckedIn" };
        private readonly IBookingService _bookingService;
        private readonly IVillaService _villaService;
        private readonly IVillaNumberService _villaNumberService;
        public BookingController(
            IWebHostEnvironment webHostEnvironment,
            IBookingService bookingService,
            IVillaService villaService,
            IVillaNumberService villaNumberService)
        {
            _webHostEnvironment = webHostEnvironment;
            _bookingService = bookingService;
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
            
            var booking = _bookingService.FinalizeBookingByUser(villaId, checkInDate, nights, userId);
           
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            _bookingService.FinalizeBooking(booking);          
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
            var bookedVillas = _bookingService.GetAllByStatus();

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
            _bookingService.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
            Response.Headers.Add("Location", session.Url);
           
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            var booking = _bookingService.GetBookingById(bookingId);
            if (booking.Status == SD.StatusPending)
            {
                //this is a pending order
                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _bookingService.BookingConfirmation(bookingId, session.Id, session.PaymentIntentId);
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
            var bookingFromDb = _bookingService.BookingDetails(bookingId);
            return View(bookingFromDb);
        }

        [HttpPost]
        //[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _bookingService.CheckIn(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _bookingService.CheckOut(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingService.CancelBooking(booking);

            TempData["Success"] = "Booking Updated Successfully.";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult GeneratePDF(int id)
        {
            string basePath = _webHostEnvironment.WebRootPath;

            // Create a new document
            WordDocument doc = new();

            // Load the template.
            string dataPathSales = basePath + @"/exports/BookingDetails.docx";
            using FileStream fileStream = new(dataPathSales, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            doc.Open(fileStream, FormatType.Automatic);

            //Get Villa Booking Details
            Booking bookingFromDb = _bookingService.GetBookingById(id);

            
            TextSelection textSelection = doc.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Name;
            
            textSelection = doc.Find("xx_customer_phone", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Phone;

            textSelection = doc.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.Email;

            textSelection = doc.Find("xx_Booking_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING DATE - " + bookingFromDb.BookingDate.ToShortDateString();

            textSelection = doc.Find("xx_BOOKING_NUMBER", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING ID: " + bookingFromDb.Id.ToString();

            textSelection = doc.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.PaymentDate.ToShortDateString();

            textSelection = doc.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckInDate.ToShortDateString();

            textSelection = doc.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.CheckOutDate.ToShortDateString();

            textSelection = doc.Find("xx_booking_total", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = bookingFromDb.TotalCost.ToString("c");

            
            TextBodyPart part = new TextBodyPart(doc);

            WTable table = new WTable(doc);

            //sets lineWidth and color

            table.TableFormat.Borders.LineWidth = 1f;

            table.TableFormat.Borders.Color = Color.Black;

            //Sets number of rows and columns.
            int rows = bookingFromDb.VillaNumber > 0 ? 3 : 2;
            table.ResetCells(rows, 4);

            //Selects the first row and appends text in each cell.

            WTableRow row0 = table.Rows[0];

            row0.Cells[0].AddParagraph().AppendText("NIGHTS");
            //row0.Cells[0].CellFormat.Borders.LineWidth = 0f;
            //row0.Cells[0].CellFormat.BackColor = Color.LightGray;


            row0.Cells[1].AddParagraph().AppendText("VILLA");
            row0.Cells[2].AddParagraph().AppendText("PRICE PER NIGHT");
            row0.Cells[3].AddParagraph().AppendText("TOTAL");
            row0.Cells[3].Width = 80;
            row0.Cells[1].Width = 220;
            row0.Cells[0].Width = 80;

            WTableRow row1 = table.Rows[1];
            row1.Cells[0].AddParagraph().AppendText(bookingFromDb.Nights.ToString());
            row1.Cells[1].AddParagraph().AppendText(bookingFromDb.Villa.Name);
            row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost/bookingFromDb.Nights).ToString("c"));
            row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("c"));
            row1.Cells[3].Width = 80;
            row1.Cells[1].Width = 220;
            row1.Cells[0].Width = 80;
            if (bookingFromDb.VillaNumber > 0)
            {
                WTableRow row2 = table.Rows[2];
                row2.Cells[1].AddParagraph().AppendText("Villa Number - "+bookingFromDb.VillaNumber.ToString());
                row2.Cells[3].Width = 80;
                row2.Cells[1].Width = 220;
                row2.Cells[0].Width = 80;
            }

            WTableStyle tableStyle = doc.AddTableStyle("CustomStyle") as WTableStyle;
            tableStyle.TableProperties.RowStripe = 1;
            tableStyle.TableProperties.ColumnStripe = 2;
            tableStyle.TableProperties.Paddings.Top = 2;
            tableStyle.TableProperties.Paddings.Bottom = 1;
            tableStyle.TableProperties.Paddings.Left = 5.4f;
            tableStyle.TableProperties.Paddings.Right = 5.4f;
            table.TableFormat.Paddings.Top = 7f;
            table.TableFormat.Paddings.Bottom = 7f;
            table.TableFormat.Borders.Horizontal.LineWidth = 1f;

            ConditionalFormattingStyle firstRowStyle = tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
            firstRowStyle.CharacterFormat.Bold = true;
            firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
            firstRowStyle.CellProperties.BackColor = Color.Black;

            table.ApplyStyle("CustomStyle");

            part.BodyItems.Add(table);

            doc.Replace("<ADDTABLEHERE>", part, false, false);
           
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
                objBookings = _bookingService.GetAll();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _bookingService.GetAll();
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

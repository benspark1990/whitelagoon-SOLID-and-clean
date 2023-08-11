using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Application.Services.SOLID.D.Interfaces;

namespace WhiteLagoon.Application.Services.SOLID.D.Implementations
{
    public class BookingService : IBookingService
    {
        /// <summary>
        /// Applaying the Dependency Inversion Principle (DIP) to the BookingService class, we need to invert the dependencies by relying on abstractions rather than concrete implementations.
        /// We'll achieve this by introducing interfaces or abstract classes to represent the dependencies.
        /// By making these changes, the BookingService class now adheres to the Dependency Inversion Principle.
        /// The high-level module, represented by BookingService, depends on the abstraction IBookingService, and it is not directly dependent on the low-level module represented by IUnitOfWork (which presumably is a concrete implementation of data access).
        /// Instead, the IUnitOfWork dependency will be injected into BookingService through its constructor, enabling loose coupling and easier testing and maintenance.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Booking FinalizeBookingByUser(int villaId, DateOnly checkInDate, int nights, string userId)
        {
            ApplicationUser user = _unitOfWork.User.Get(u => u.Id == userId);

            Booking booking = new()
            {
                Villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name,
            };
            booking.TotalCost = booking.Villa.Price * nights;

            return booking;
        }

        public void FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);

            booking.TotalCost = (villa.Price * booking.Nights);
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            _unitOfWork.Booking.UpdateStripePaymentID(id, sessionId, paymentIntentId);
            _unitOfWork.Save();
        }

        public void BookingConfirmation(int bookingId, string sessionId, string sessionPaymentIntentId)
        {
            _unitOfWork.Booking.UpdateStripePaymentID(bookingId, sessionId, sessionPaymentIntentId);
            _unitOfWork.Booking.UpdateStatus(bookingId, SD.StatusApproved, 0);
            _unitOfWork.Save();
        }

        public Booking BookingDetails(int bookingId)
        {
            Booking bookingFromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");
            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                var availableVillaNumbers = AssignAvailableVillaNumberByVilla(bookingFromDb.VillaId, bookingFromDb.CheckInDate);

                bookingFromDb.VillaNumbers = _unitOfWork.VillaNumber.GetAll().Where(m => m.VillaId == bookingFromDb.VillaId
                            && availableVillaNumbers.Any(x => x == m.Villa_Number)).ToList();
            }
            else
            {
                bookingFromDb.VillaNumbers = _unitOfWork.VillaNumber.GetAll().Where(m => m.VillaId == bookingFromDb.VillaId && m.Villa_Number == bookingFromDb.VillaNumber).ToList();
            }

            return bookingFromDb;
        }

        public void CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
            _unitOfWork.Save();
        }

        public void CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.VillaNumber);
            _unitOfWork.Save();
        }

        public void CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, 0);
            _unitOfWork.Save();
        }

        public List<Booking> GetAllByStatus()
        {
            return _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();
        }

        public Booking GetBookingById(int bookingId)
        {
            return _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Villa");
        }

        public List<int> AssignAvailableVillaNumberByVilla(int villaId, DateOnly checkInDate)
        {
            List<int> availableVillaNumbers = new List<int>();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll().Where(m => m.VillaId == villaId).ToList();

            var checkedInVilla = _unitOfWork.Booking.GetAll().Where(m => m.Status == SD.StatusCheckedIn && m.VillaId == villaId).Select(u => u.VillaNumber);

            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    //Villa is not checked in
                    availableVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return availableVillaNumbers;
        }

        public List<Booking> GetAll()
        {
            return _unitOfWork.Booking.GetAll(includeProperties: "User,Villa").ToList();
        }

        public WordDocument GeneratePDF(int id, string basePath)
        {
            WordDocument doc = new();

            // Load the template.
            string dataPathSales = basePath + @"/exports/BookingDetails.docx";
            using FileStream fileStream = new(dataPathSales, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            doc.Open(fileStream, FormatType.Automatic);

            //Get Villa Booking Details
            Booking bookingFromDb = GetBookingById(id);


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

            table.TableFormat.Borders.Color = Syncfusion.Drawing.Color.Black;

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
            row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost / bookingFromDb.Nights).ToString("c"));
            row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("c"));
            row1.Cells[3].Width = 80;
            row1.Cells[1].Width = 220;
            row1.Cells[0].Width = 80;
            if (bookingFromDb.VillaNumber > 0)
            {
                WTableRow row2 = table.Rows[2];
                row2.Cells[1].AddParagraph().AppendText("Villa Number - " + bookingFromDb.VillaNumber.ToString());
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
            firstRowStyle.CharacterFormat.TextColor = Syncfusion.Drawing.Color.FromArgb(255, 255, 255, 255);
            firstRowStyle.CellProperties.BackColor = Syncfusion.Drawing.Color.Black;

            table.ApplyStyle("CustomStyle");

            part.BodyItems.Add(table);

            doc.Replace("<ADDTABLEHERE>", part, false, false);

            return doc;
        }
    }
}

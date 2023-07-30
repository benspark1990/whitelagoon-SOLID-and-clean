using Microsoft.AspNetCore.Hosting;
using System.IO;
using WhiteLagoon.Application.Services.SOLID.S.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.S.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public ImageService(string productPath)
        {
        }

        public string SaveImage(Villa villa)
        {
            if (villa.Image is not null)
            {
                string productPath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\products");

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    villa.Image.CopyTo(fileStream);
                }

                return @"\images\products\" + fileName;
            }

            return "https://placehold.co/600x400";
        }

        public void DeleteImage(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                FileInfo file = new(oldImagePath);

                if (file.Exists)
                {
                    file.Delete();
                }
            }
        }
    }

}

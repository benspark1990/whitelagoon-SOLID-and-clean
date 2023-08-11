using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.SOLID.S.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.S.Implementations
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;

        /// <summary>
        /// The Single Responsibility Principle states that a class should have only one reason to change, meaning it should have a single responsibility or job. 
        /// Create a separate class to handle image-related operations, such as saving and deleting images
        /// By refactoring the code in this way, we've separated the image-related operations into a new ImageService class, which adheres to the Single Responsibility Principle.
        /// The VillaService class now focuses solely on handling villa-related operations and delegates the image-related tasks to the ImageService class.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="imageService"></param>
        public VillaService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public Villa GetById(int id)
        {
            return _unitOfWork.Villa.Get(u => u.Id == id);
        }

        public List<Villa> GetAll(string? includeProperties = null)
        {
            return _unitOfWork.Villa.GetAll(includeProperties: includeProperties).ToList();
        }

        public void Create(Villa villa, string path)
        {
            if (villa is null)
                return;

            villa.ImageUrl = _imageService.SaveImage(villa);
            _unitOfWork.Villa.Add(villa);
            _unitOfWork.Save();
        }

        public void Update(Villa villa, string path)
        {
            if (villa is null)
                return;

            _imageService.DeleteImage(villa.ImageUrl);
            villa.ImageUrl = _imageService.SaveImage(villa);
            _unitOfWork.Villa.Update(villa);
            _unitOfWork.Save();
        }

        public void Delete(Villa villa, string path)
        {
            if (villa is null)
                return;

            _imageService.DeleteImage(villa.ImageUrl);
            _unitOfWork.Villa.Remove(villa);
            _unitOfWork.Save();
        }
    }
}

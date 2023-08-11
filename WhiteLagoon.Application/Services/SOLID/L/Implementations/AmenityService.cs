using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.SOLID.L.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.L.Implementations
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// To make the AmenityService class adhere to the Liskov Substitution Principle (LSP),
        /// we can refactor the code by introducing a common base interface for all services.
        /// This base interface will define generic CRUD (Create, Read, Update, Delete) operations that all services should implement.
        /// Then, IAmenityService will extend this base interface, and AmenityService will implement the extended interface.
        /// By introducing the IService<T> interface as the base for all services, we establish a common contract that every service should follow,
        /// ensuring that each service can be used interchangeably wherever the base interface is expected. This adheres to the Liskov Substitution Principle, 
        /// as the AmenityService now properly implements the IAmenityService interface and satisfies the behavior defined in the base IService<Amenity> interface.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Amenity Get(int id)
        {
            return _unitOfWork.Amenity.Get(u => u.Id == id);
        }

        public List<Amenity> GetAll(string? includeProperties = null)
        {
            return _unitOfWork.Amenity.GetAll(includeProperties: includeProperties).ToList();
        }

        public void Create(Amenity amenity)
        {
            if (amenity is null)
                return;

            _unitOfWork.Amenity.Add(amenity);
            _unitOfWork.Save();
        }

        public void Update(Amenity amenity)
        {
            if (amenity is null)
                return;

            _unitOfWork.Amenity.Update(amenity);
            _unitOfWork.Save();
        }

        public void Delete(Amenity amenity)
        {
            if (amenity is null)
                return;

            _unitOfWork.Amenity.Remove(amenity);
            _unitOfWork.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Services.SOLID.O.Interfaces;
using WhiteLagoon.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhiteLagoon.Application.Services.SOLID.O.Implementations
{
    /// <summary>
    /// To make the VillaNumberService adhere to the Open/Closed Principle (OCP), we can use the Repository Pattern.
    /// We'll create an interface for the IVillaNumberRepository and use it in the VillaNumberService.
    /// This way, the VillaNumberService will depend on abstractions (interfaces) rather than concrete implementations,
    /// allowing us to extend its behavior without modifying the existing code.
    /// By following the Open/Closed Principle using the Repository Pattern,
    /// we've made the VillaNumberService open for extension by depending on the IVillaNumberRepository interface.
    /// This allows us to add new database access strategies or repositories without modifying the VillaNumberService class,
    /// promoting better maintainability and flexibility in the codebase.
    /// </summary>
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IVillaNumberRepository _villaNumberRepository;
        public VillaNumberService(IVillaNumberRepository villaNumberRepository)
        {

            _villaNumberRepository = villaNumberRepository;

        }
        public void AddVillaNumber(VillaNumber villaNumber)
        {
            _villaNumberRepository.Create(villaNumber);
        }

        public VillaNumber GetVillaNumber(int number)
        {
            return _villaNumberRepository.Get(number);
        }

        public List<VillaNumber> GetAllVillaNumber(string? includeProperties = null)
        {
            return _villaNumberRepository.GetAll(includeProperties);
        }

        public List<VillaNumber> GetVillaNumberByNumber(int number)
        {
            return _villaNumberRepository.GetByNumber(number);
        }

        public void RemoveVillaNumber(VillaNumber villaNumber)
        {
            _villaNumberRepository.Delete(villaNumber);
        }

        public void UpdateVillaNumber(VillaNumber villaNumber)
        {
            _villaNumberRepository.Update(villaNumber);
        }
    }
}

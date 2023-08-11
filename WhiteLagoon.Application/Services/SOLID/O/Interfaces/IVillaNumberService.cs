using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.O.Interfaces
{
    public interface IVillaNumberService
    {
        VillaNumber GetVillaNumber(int number);
        List<VillaNumber> GetAllVillaNumber(string? includeProperties = null);
        List<VillaNumber> GetVillaNumberByNumber(int number);
        void AddVillaNumber(VillaNumber villaNumber);
        void UpdateVillaNumber(VillaNumber villaNumber);
        void RemoveVillaNumber(VillaNumber villaNumber);
    }
}

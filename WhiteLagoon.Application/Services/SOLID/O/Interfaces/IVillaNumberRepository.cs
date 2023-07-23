using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.O.Interfaces
{
    public interface IVillaNumberRepository
    {
        VillaNumber Get(int number);
        List<VillaNumber> GetAll(string? includeProperties = null);
        List<VillaNumber> GetByNumber(int number);
        void Create(VillaNumber villaNumber);
        void Update(VillaNumber villaNumber);
        void Delete(VillaNumber villaNumber);
    }
}

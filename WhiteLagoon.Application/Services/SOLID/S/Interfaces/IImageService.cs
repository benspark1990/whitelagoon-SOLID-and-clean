using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.S.Interfaces
{
    public interface IImageService
    {
        string SaveImage(Villa villa);
        void DeleteImage(string imageUrl);

    }
}

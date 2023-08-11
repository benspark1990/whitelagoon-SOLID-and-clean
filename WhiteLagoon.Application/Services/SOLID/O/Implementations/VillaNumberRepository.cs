﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Application.Services.SOLID.O.Implementations
{
    public class VillaNumberRepository : Interfaces.IVillaNumberRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public VillaNumberRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public VillaNumber Get(int number)
        {
            return _unitOfWork.VillaNumber.Get(u => u.Villa_Number == number);
        }

        public List<VillaNumber> GetAll(string? includeProperties = null)
        {
            return _unitOfWork.VillaNumber.GetAll(includeProperties: includeProperties).ToList();
        }

        public List<VillaNumber> GetByNumber(int number)
        {
            return _unitOfWork.VillaNumber.GetAll(u => u.Villa_Number == number).ToList();
        }

        public void Create(VillaNumber villaNumber)
        {
            if (villaNumber is null)
                return;

            _unitOfWork.VillaNumber.Add(villaNumber);
            _unitOfWork.Save();
        }

        public void Update(VillaNumber villaNumber)
        {
            if (villaNumber == null)
                return;

            _unitOfWork.VillaNumber.Update(villaNumber);
            _unitOfWork.Save();
        }

        public void Delete(VillaNumber villaNumber)
        {
            if (villaNumber == null)
                return;

            _unitOfWork.VillaNumber.Remove(villaNumber);
            _unitOfWork.Save();
        }
    }
}

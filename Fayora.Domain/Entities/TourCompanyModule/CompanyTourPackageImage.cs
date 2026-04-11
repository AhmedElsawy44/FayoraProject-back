using Fayora.Domain.Entities.SharedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.TourCompanyModule
{

    public class CompanyTourPackageImage : TourPackageImageBase
    {
        public CompanyTourPackageImage(Guid packageId, string imageUrl)
            : base(packageId, imageUrl) { }
        private CompanyTourPackageImage() { }
    }

}

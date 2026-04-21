using Fayora.Domain.Common.Entity;
using Fayora.Domain.Common.Results;
using Fayora.Domain.Enums.TourCompanyModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fayora.Domain.Entities.TourCompanyModule
{
    public class TourCompany : AuditableEntity<Guid>
    {
        public Guid UserId { get; init; }
        public string CompanyName { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public string CommercialRegisterNumber { get; private set; } = null!;
        public string TaxRegistrationNumber { get; private set; } = null!;
        public string? LogoUrl { get; private set; }
        public string CurrencyCode { get; private set; } = null!;
        public float Rating { get; private set; }
        public int ReviewCount { get; private set; }
        public int CompletedToursCount { get; private set; }
        public TourCompanyStatus Status { get; private set; }
        public bool IsListingEnabled { get; private set; }

        private readonly List<CompanyTourPackage> _packages = [];
        public IReadOnlyCollection<CompanyTourPackage> Packages => _packages.AsReadOnly();

        private TourCompany(
            Guid userId,
            string companyName,
            string description,
            string commercialRegisterNumber,
            string taxRegistrationNumber,
            string currencyCode = "EGP")
        {
            UserId = userId;
            CompanyName = companyName;
            Description = description;
            CommercialRegisterNumber = commercialRegisterNumber;
            TaxRegistrationNumber = taxRegistrationNumber;
            CurrencyCode = currencyCode;
            Rating = 0f;
            ReviewCount = 0;
            CompletedToursCount = 0;
            Status = TourCompanyStatus.Pending;
            IsListingEnabled = false;
        }

        private TourCompany()
        {
            CurrencyCode = string.Empty;
        }

        public static Result<TourCompany> Create(
            Guid userId,
            string companyName,
            string description,
            string commercialRegisterNumber,
            string taxRegistrationNumber,
            string currencyCode = "EGP")
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return Error.Validation("TourCompany.EmptyName", "Company name cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                return Error.Validation("TourCompany.EmptyDescription", "Description cannot be empty.");

            if (string.IsNullOrWhiteSpace(commercialRegisterNumber))
                return Error.Validation("TourCompany.EmptyRegisterNumber", "Commercial register number cannot be empty.");

            if (string.IsNullOrWhiteSpace(taxRegistrationNumber))
                return Error.Validation("TourCompany.EmptyTaxNumber", "Tax registration number cannot be empty.");

            return new TourCompany(userId, companyName, description,
                commercialRegisterNumber, taxRegistrationNumber, currencyCode);
        }

        public Result<Success> UpdateDetails(string companyName, string description, string? websiteUrl)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                return Error.Validation("TourCompany.EmptyName", "Company name cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                return Error.Validation("TourCompany.EmptyDescription", "Description cannot be empty.");

            CompanyName = companyName;
            Description = description;
            Updated();
            return Result.Success;
        }

        public Result<Success> SetLogo(string logoUrl)
        {
            if (string.IsNullOrWhiteSpace(logoUrl))
                return Error.Validation("TourCompany.EmptyLogo", "Logo URL cannot be empty.");

            LogoUrl = logoUrl;
            Updated();
            return Result.Success;
        }

        public Result<Success> UpdateStatus(TourCompanyStatus newStatus)
        {
            if (newStatus == TourCompanyStatus.Pending)
                return Error.Validation("TourCompany.InvalidStatus", "Cannot set status back to Pending.");

            Status = newStatus;
            IsListingEnabled = newStatus == TourCompanyStatus.Active;
            Updated();
            return Result.Success;
        }

        public void UpdateRating(float newRating)
        {
            Rating = (Rating * ReviewCount + newRating) / (ReviewCount + 1);
            ReviewCount++;
            Updated();
        }

        public void IncrementCompletedTours()
        {
            CompletedToursCount++;
            Updated();
        }
    }
}

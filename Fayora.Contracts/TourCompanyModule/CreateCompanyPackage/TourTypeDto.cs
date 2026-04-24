namespace Fayora.Contracts.TourCompanyModule.CreateCompanyPackage
{
    [Flags]
    public enum TourTypeDto
    {
        None = 0,
        Cultural = 1,
        Adventure = 2,
        Historical = 4,
        Nature = 8,
        Food = 16,
        Religious = 32,
        Beach = 64
    }
}

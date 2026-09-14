namespace Zukya.Api.Modules.Users.Inputs;

public class UpdateUserApiInput
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? TaxId { get; set; }

    public UpdateUserApiInput(
        string? name,
        string? phone,
        string? taxId
    )
    {
        Name = name;
        Phone = phone;
        TaxId = taxId;
    }
}

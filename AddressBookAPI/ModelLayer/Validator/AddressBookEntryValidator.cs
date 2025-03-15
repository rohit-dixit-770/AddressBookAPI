using FluentValidation;
using ModelLayer.Model;

namespace ModelLayer.Validator
{
    /// <summary>
    /// Validator for AddressBookEntryDTO using FluentValidation.
    /// </summary>
    public class AddressBookEntryValidator : AbstractValidator<AddressBookEntry>
    {
        public AddressBookEntryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be exactly 10 digits");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is Required")
                .When(x => !string.IsNullOrEmpty(x.Address)); 
        }
    }
}

using System.ComponentModel.DataAnnotations;
namespace RailConnect.Models
{

    public class ExactLengthAttribute : ValidationAttribute
    {
        private readonly int _length;

        public ExactLengthAttribute(int length)
        {
            _length = length;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int intValue)
            {
                if (Math.Floor(Math.Log10(intValue) + 1) == _length)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(ErrorMessage ?? $"Field must be exactly {_length} digits long.");
        }
    }

}

using System;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date >= DateTime.Now.AddDays(3))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Datum termina mora biti postavljen najmanje 3 dana unaprijed!");
                }
            }
            return new ValidationResult("Invalid date format.");
        }
    }
}

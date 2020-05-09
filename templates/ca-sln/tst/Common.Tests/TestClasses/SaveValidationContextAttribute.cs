using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Tests.TestClasses
{
    public class SaveValidationContextAttribute : ValidationAttribute
    {
        public static IList<ValidationContext> SavedContexts = new List<ValidationContext>();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            SavedContexts.Add(validationContext);
            return ValidationResult.Success;
        }
    }
}

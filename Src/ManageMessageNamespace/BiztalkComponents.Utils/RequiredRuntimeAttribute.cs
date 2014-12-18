using System.ComponentModel.DataAnnotations;

namespace BizTalkComponents.Utils
{
    public class RequiredRuntimeAttribute : ValidationAttribute
    {
        private const string ErrorMsg = "Property must be set.";

        public RequiredRuntimeAttribute()
            : base(ErrorMsg){}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var items = validationContext.Items;

            if (items.ContainsKey("IsRuntime"))
            {
                if (value == null)
                {
                    return new ValidationResult(ErrorMsg, new[] { validationContext.MemberName });
                }
                
                if (value is string)
                {
                    if (string.IsNullOrEmpty(value as string))
                    {
                        return new ValidationResult(ErrorMsg, new[] {validationContext.MemberName});
                    }
                }
            }

            return null;
        }
    }
}
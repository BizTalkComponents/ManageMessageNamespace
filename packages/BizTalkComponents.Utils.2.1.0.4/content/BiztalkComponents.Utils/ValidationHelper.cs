using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BizTalkComponents.Utils
{
    public class ValidationHelper
    {
        public static IEnumerable<string> Validate(object instance, bool isRuntime)
        {
            ValidationContext vc;

            if (isRuntime)
            {
                var dictionary = new Dictionary<object, object> {{"IsRuntime", "true"}};
                vc = new ValidationContext(instance, null, dictionary);
            }
            else
            {
                vc = new ValidationContext(instance, null, null);
            }

            var result = new List<ValidationResult>();

            Validator.TryValidateObject(instance, vc, result, true);

            foreach (var error in result)
            {
                foreach (var e in error.MemberNames)
                {
                    yield return string.Format("{0}: {1}", e, error.ErrorMessage);
                }
            }
        }
    }
}
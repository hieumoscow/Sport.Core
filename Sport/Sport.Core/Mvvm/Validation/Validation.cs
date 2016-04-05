using System;
using System.Text.RegularExpressions;
using Sport.Core.Strings;

namespace Sport.Core.Mvvm.Validation
{
    public class Validation
    {
        

        public Func<string, bool> Validate { get; }
        public string Message { get; }

        public Validation(Func<string, bool> func, string message)
        {
            Validate = func;
            Message = message;
        }

        public static Validation CreateRequiredValidation(string message = null)
        {
            return new Validation(val => !string.IsNullOrEmpty(val), message ?? AppResources.Validation_Required);
        }
        public static Validation CreateRegexValidation(string regex, string message = null)
        {

            return new Validation(val => Regex.IsMatch(val, regex), message ?? AppResources.Validation_Required);
        }
    }
}
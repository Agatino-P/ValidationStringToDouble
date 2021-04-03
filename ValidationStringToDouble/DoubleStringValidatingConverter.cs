using FluentValidation;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace ValidationStringToDouble
{
    public class DoubleStringValidatingConverter : AbstractValidator<string>
    {
        public double ConvertedValue { get; private set; }
        public DoubleStringValidatingConverter()
        {
            //this.CascadeMode = CascadeMode.Stop;

            this.RuleFor(s => s)
                .NotEmpty().WithMessage("Must Not Be Empty");

            this.RuleFor(s => s)
                .Must(text => regexProcessing(text)).WithMessage("Only leading sign, digits and decimal separator allowed");
        }

        private bool regexProcessing(string text)
        {
            var matches = Regex.Match(text, @"^([\-]?\d+)(.?)(\d*)");

            string separator = matches.Groups[2].Value;

            if (separator==string.Empty)
            {
                if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value)) { return false; }
                if (double.IsNaN(value) || double.IsInfinity(value)) { return false; }
                ConvertedValue = value;
                return true;
            }

            if (separator == ".")
            {
                if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value)) { return false; }
                if (double.IsNaN(value) || double.IsInfinity(value)) { return false; }
                ConvertedValue = value;
                return true;
            }

            string cultureSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (separator == cultureSeparator)
            {
                string normalizedText = text.Replace(cultureSeparator, ".");
                if (!double.TryParse(normalizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out double value)) { return false; }
                if (double.IsNaN(value) || double.IsInfinity(value)) { return false; }
                ConvertedValue = value;
                return true;
            }

            return false;
        }

    }
}

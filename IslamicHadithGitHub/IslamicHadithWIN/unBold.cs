using System;
using System.Text.RegularExpressions;

namespace IslamicHadithWIN
{
    public class unBold
    {
        public string unBoldHadith(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}

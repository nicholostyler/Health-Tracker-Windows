using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Health_Track.Helpers
{
    public static class FieldVerifier
    {

        public static bool ValidateName(string name)
        {
            // check if the stirng is empty
            if (string.IsNullOrEmpty(name)) return false;

            // check if there is a digit in the string
            bool isAllLetters =  name.All(char.IsLetter);
            if (!isAllLetters) return false;

            return true;
        }

        public static bool ValidateWeight(string weight)
        {
            double currentWeight = 0.0;
            bool test = weight.All(char.IsDigit);
             
            if (!test) return false;

            if (double.TryParse(weight, out currentWeight))
            {
                return true;
            }

            return true;
        }

        public static bool ValidateDate(DateTimeOffset date)
        {
            // Bug - Selected Date shows correctly on view but is 1600s?
            if (date.Year == 1600) return true;

            // set today's date and the selectedDate value
            DateTimeOffset today = DateTime.Now;
            //date = pickerDate.SelectedDate;
            // make sure the nullable value has a value
            //if (date.HasValue) return false;
            // if the value of the compareTo is greater than zero it is after today.
            if (date.CompareTo(today) > 0) return false;

            return true;
        }
    }
}

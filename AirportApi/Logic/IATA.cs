using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace AirportApi.Logic {

    public class IATA {
        public string Code {get;}

        public static Result<IATA> fromString(string input) {
            input = Regex.Replace(input, "[^a-zA-Z]", "").ToUpper();
            if (input.Length != 3) {
                return Result.Fail<IATA>($"Invalid IATA format {input}");
            }
            return Result.Ok(new IATA(input));
        }

        private IATA(string input) {
            this.Code = input;
        }

    }

}
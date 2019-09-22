using System;
using CSharpFunctionalExtensions;
using GeoCoordinatePortable;
using System.Text.RegularExpressions;

namespace AirportApi.Logic {

    public class Location {
        public readonly double Lat;
        public readonly double Lon;

        private Location(double lat, double lon) {
            Lat = lat;
            Lon = lon;
        }

        private GeoCoordinate toGeoCoordinate() {
            return new GeoCoordinate(this.Lat, this.Lon);
        }

        static public Result<Location> FromDoubles(double lat, double lon) {
            if(Math.Abs(lat) > 60) {
                return Result.Fail<Location>($"Invalid lat {lat}");
            }
            if(Math.Abs(lon) > 180) {
                return Result.Fail<Location>($"Invalid lon {lon}");
            }
            return Result.Ok(new Location(lat, lon));
        }

        static public Result<Location> FromString(string str) {
            try {
                var strValues = Regex.Replace(str, "\\(|\\)", "").Split(';');
                double lat = System.Convert.ToDouble(strValues[0]);
                double lon = System.Convert.ToDouble(strValues[1]);
                return Location.FromDoubles(lat, lon);
            } catch(Exception e) {
                return Result.Fail<Location>(e.Message);
            }
        }

        public override string ToString() {
            return $"({Lat};{Lon})";
        }

        /// <summary>
        /// The Haversine formula is used to calculate the distance. 
        /// The Haversine formula accounts for the curvature of the earth, 
        /// but assumes a spherical earth rather than an ellipsoid. For long distances, 
        /// the Haversine formula introduces an error of less than 0.1 percent.
        /// Altitude is not used to calculate the distance.
        /// Check the documentation for GeoCoordinate in .NET 4.8 for more details.
        /// </summary>
        public double DistanceMeters(Location other) {
            return this.toGeoCoordinate().GetDistanceTo(other.toGeoCoordinate());
        }

        public double DistanceMiles(Location other) {
            return DistanceMeters(other)*0.0006213712d;
        }


    }

}
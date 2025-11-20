using System;
using System.Collections.Generic;

namespace BadUnitConverter
{
    public class Converter
    {
        public class UnitConverter
        {
            public double ConvertDistance(double value, string fromUnit, string toUnit)
            {
                if (fromUnit == "km" && toUnit == "miles")
                {
                    return value * 0.621371;
                }
                else if (fromUnit == "miles" && toUnit == "km")
                {
                    return value * 1.60934;
                }
                else if (fromUnit == "meters" && toUnit == "feet")
                {
                    return value * 3.28084; 
                }
                else if (fromUnit == "feet" && toUnit == "meters")
                {
                    return value * 0.3048; 
                }
                else if (fromUnit == "hectares" && toUnit == "acres")
                {
                    return value * 2.47105; 
                }
                else if (fromUnit == "acres" && toUnit == "hectares")
                {
                    return value * 0.404686; 
                }
                else
                {
                    return -1; 
                }
            }

            public string ConvertAndFormat(double value, string fromUnit, string toUnit)
            {
                double result = ConvertDistance(value, fromUnit, toUnit);
                if (result == -1) return "Error";

                string formatted = $"{value} {fromUnit} = {result:F4} {toUnit}";

                Console.WriteLine($"Converted: {formatted}");

                return formatted;
            }

            public void ProcessAllConversions()
            {
                List<double> values = new List<double>() { 1, 5, 10, 20, 50 }; 

                foreach (double val in values)
                {
                    string[] units = { "km", "miles", "meters", "feet", "hectares", "acres" };

                    for (int i = 0; i < units.Length; i++)
                    {
                        for (int j = 0; j < units.Length; j++)
                        {
                            if (i != j)
                            {
                                double r = ConvertDistance(val, units[i], units[j]);
                                if (r != -1) 
                                {
                                    Console.WriteLine($"{val} {units[i]} -> {r:F2} {units[j]}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public class Calc
        {
            public double x; 
            public double y; 
            public List<string> l = new List<string>(); 

            public double m1(double a, string b) 
            {
                if (b == "km2miles") return a * 0.621371;
                if (b == "miles2km") return a * 1.60934;
                return 0;
            }
        }

        public class DistanceCalculator
        {
            public double Calculate(string unit, double value)
            {
                switch (unit)
                {
                    case "km": return value;
                    case "miles": return value * 1.60934;
                    case "meters": return value / 1000;
                    case "feet": return value * 0.0003048;
                    default: return 0;
                }
            }
        }

        public class User
        {
            public Settings Settings { get; set; }
        }

        public class Settings
        {
            public UnitPreferences Preferences { get; set; }
        }

        public class UnitPreferences
        {
            public string DefaultUnit { get; set; }
        }

        public void BadMethod()
        {
            User user = new User();
            string defaultUnit = user.Settings.Preferences.DefaultUnit.ToUpper();
        }
    }
}
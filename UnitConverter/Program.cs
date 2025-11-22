using System;
using System.Collections.Generic;

namespace BadUnitConverter
{
    public static class ConversionConstants
    {
        public const double KilometersToMiles = 0.631371;
        public const double MilesToKilometers = 1.60934;

        public const double MetersToFeet = 0.3048;    
        public const double FeetToMeters = 3.28084;  

        public const double HectaresToAcres = 2.47105;
        public const double AcresToHectares = 0.404686;

        public const int MetersInKilometer = 1000;
        public const int DecimalPrecision = 4;
    }

    public enum DistanceUnit
    {
        Kilometers,
        Miles,
        Meters,
        Feet
    }

    public enum AreaUnit
    {
        Hectares,
        Acres
    }

    public class InvalidConversionException : Exception
    {
        public InvalidConversionException(string message) : base(message) { }
    }

    public interface IUnitConverter<T>
    {
        double Convert(double value, T fromUnit, T toUnit);
        bool CanConvert(T fromUnit, T toUnit);
    }

    public interface IResultFormatter
    {
        string FormatSuccess(double value, string fromUnit, double result, string toUnit);
        string FormatError(string fromUnit, string toUnit);
    }

    public class DistanceConverter : IUnitConverter<DistanceUnit>
    {
        public double Convert(double value, DistanceUnit fromUnit, DistanceUnit toUnit)
        {
            if (!CanConvert(fromUnit, toUnit))
            {
                throw new InvalidConversionException($"Conversion from {fromUnit} to {toUnit} is not supported");
            }

            if (fromUnit == toUnit) return value;

            double valueInKilometers = ConvertToKilometers(value, fromUnit);
            return ConvertFromKilometers(valueInKilometers, toUnit);
        }

        public bool CanConvert(DistanceUnit fromUnit, DistanceUnit toUnit)
        {
            return fromUnit == toUnit;
        }

        private double ConvertToKilometers(double value, DistanceUnit unit)
        {
            return unit switch
            {
                DistanceUnit.Kilometers => value,
                DistanceUnit.Miles => value * ConversionConstants.MilesToKilometers,
                DistanceUnit.Meters => value / ConversionConstants.MetersInKilometer,
                DistanceUnit.Feet => value * ConversionConstants.FeetToMeters / ConversionConstants.MetersInKilometer,
                _ => throw new ArgumentException($"Unsupported distance unit: {unit}")
            };
        }

        private double ConvertFromKilometers(double valueInKilometers, DistanceUnit targetUnit)
        {
            return targetUnit switch
            {
                DistanceUnit.Kilometers => valueInKilometers,
                DistanceUnit.Miles => valueInKilometers * ConversionConstants.KilometersToMiles,
                DistanceUnit.Meters => valueInKilometers * ConversionConstants.MetersInKilometer,
                DistanceUnit.Feet => valueInKilometers * ConversionConstants.MetersInKilometer / ConversionConstants.FeetToMeters,
                _ => throw new ArgumentException($"Unsupported distance unit: {targetUnit}")
            };
        }
    }

    public class AreaConverter : IUnitConverter<AreaUnit>
    {
        public double Convert(double value, AreaUnit fromUnit, AreaUnit toUnit)
        {
            if (!CanConvert(fromUnit, toUnit))
            {
                throw new InvalidConversionException($"Conversion from {fromUnit} to {toUnit} is not supported");
            }

            if (fromUnit == toUnit) return value;

            return fromUnit switch
            {
                AreaUnit.Hectares when toUnit == AreaUnit.Acres => value * ConversionConstants.HectaresToAcres,
                AreaUnit.Acres when toUnit == AreaUnit.Hectares => value * ConversionConstants.AcresToHectares,
                _ => throw new InvalidConversionException($"Unsupported area conversion: {fromUnit} to {toUnit}")
            };
        }

        public bool CanConvert(AreaUnit fromUnit, AreaUnit toUnit)
        {
            return fromUnit != toUnit;
        }
    }

    public class ConsoleFormatter : IResultFormatter
    {
        public string FormatSuccess(double value, string fromUnit, double result, string toUnit)
        {
            return $"{value} {fromUnit} = {result:F2} {toUnit}";
        }

        public string FormatError(string fromUnit, string toUnit)
        {
            return $"Error: Cannot convert from {fromUnit} to {toUnit}";
        }
    }

    public class OutputService
    {
        private readonly IResultFormatter _formatter;

        public OutputService(IResultFormatter formatter)
        {
            _formatter = formatter;
        }

        public void DisplayConversion(double value, string fromUnit, double result, string toUnit)
        {
            string formattedResult = _formatter.FormatSuccess(value, fromUnit, result, toUnit);
            Console.WriteLine(formattedResult);
        }

        public void DisplayError(string errorMessage)
        {
            Console.WriteLine($"Error: {errorMessage}");
        }

        public void LogConversion(string conversionDetails)
        {
            Console.WriteLine($"[LOG] {conversionDetails}");
        }
    }

    public class ConversionService
    {
        private readonly IUnitConverter<DistanceUnit> _distanceConverter;
        private readonly IUnitConverter<AreaUnit> _areaConverter;
        private readonly OutputService _outputService;

        public ConversionService(
            IUnitConverter<DistanceUnit> distanceConverter,
            IUnitConverter<AreaUnit> areaConverter,
            OutputService outputService)
        {
            _distanceConverter = distanceConverter;
            _areaConverter = areaConverter;
            _outputService = outputService;
        }

        public double ConvertDistance(double value, DistanceUnit fromUnit, DistanceUnit toUnit)
        {
            try
            {
                double result = _distanceConverter.Convert(value, fromUnit, toUnit);
                _outputService.LogConversion($"Converted {value} {fromUnit} to {result} {toUnit}");
                return result;
            }
            catch (InvalidConversionException ex)
            {
                _outputService.DisplayError(ex.Message);
                throw;
            }
        }

        public double ConvertArea(double value, AreaUnit fromUnit, AreaUnit toUnit)
        {
            try
            {
                double result = _areaConverter.Convert(value, fromUnit, toUnit);
                _outputService.LogConversion($"Converted {value} {fromUnit} to {result} {toUnit}");
                return result;
            }
            catch (InvalidConversionException ex)
            {
                _outputService.DisplayError(ex.Message);
                throw;
            }
        }

        public void DisplayFormattedConversion(double value, string fromUnit, double result, string toUnit)
        {
            _outputService.DisplayConversion(value, fromUnit, result, toUnit);
        }
    }

    public class BatchConversionService
    {
        private readonly ConversionService _conversionService;
        private readonly OutputService _outputService;

        public BatchConversionService(ConversionService conversionService, OutputService outputService)
        {
            _conversionService = conversionService;
            _outputService = outputService;
        }

        public void ProcessDistanceConversions()
        {
            double[] testValues = { 1, 5, 10, 20, 50 };
            DistanceUnit[] distanceUnits = { DistanceUnit.Kilometers, DistanceUnit.Miles, DistanceUnit.Meters, DistanceUnit.Feet };

            foreach (double value in testValues)
            {
                foreach (DistanceUnit sourceUnit in distanceUnits)
                {
                    foreach (DistanceUnit targetUnit in distanceUnits)
                    {
                        if (sourceUnit != targetUnit)
                        {
                            try
                            {
                                double result = _conversionService.ConvertDistance(value, sourceUnit, targetUnit);
                                _outputService.DisplayConversion(value, sourceUnit.ToString(), result, targetUnit.ToString());
                            }
                            catch (InvalidConversionException)
                            {
                                
                            }
                        }
                    }
                }
            }
        }

        public void ProcessAreaConversions()
        {
            double[] testValues = { 1, 5, 10, 20, 50 };
            AreaUnit[] areaUnits = { AreaUnit.Hectares, AreaUnit.Acres };

            foreach (double value in testValues)
            {
                foreach (AreaUnit sourceUnit in areaUnits)
                {
                    foreach (AreaUnit targetUnit in areaUnits)
                    {
                        if (sourceUnit != targetUnit)
                        {
                            try
                            {
                                double result = _conversionService.ConvertArea(value, sourceUnit, targetUnit);
                                _outputService.DisplayConversion(value, sourceUnit.ToString(), result, targetUnit.ToString());
                            }
                            catch (InvalidConversionException)
                            {
                                
                            }
                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var distanceConverter = new DistanceConverter();
            var areaConverter = new AreaConverter();
            var formatter = new ConsoleFormatter();
            var outputService = new OutputService(formatter);
            var conversionService = new ConversionService(distanceConverter, areaConverter, outputService);
            var batchService = new BatchConversionService(conversionService, outputService);

            Console.WriteLine("=== Unit Converter (With Bugs) ===");

            Console.WriteLine("\n--- Distance Conversions ---");
            double kmToMiles = conversionService.ConvertDistance(10, DistanceUnit.Kilometers, DistanceUnit.Miles);
            conversionService.DisplayFormattedConversion(10, "km", kmToMiles, "miles");

            double metersToFeet = conversionService.ConvertDistance(5, DistanceUnit.Meters, DistanceUnit.Feet);
            conversionService.DisplayFormattedConversion(5, "meters", metersToFeet, "feet");

            Console.WriteLine("\n--- Area Conversions ---");
            double hectaresToAcres = conversionService.ConvertArea(2, AreaUnit.Hectares, AreaUnit.Acres);
            conversionService.DisplayFormattedConversion(2, "hectares", hectaresToAcres, "acres");

            Console.WriteLine("\n--- Batch Processing ---");
            Console.WriteLine("Processing distance conversions...");
            batchService.ProcessDistanceConversions();

            Console.WriteLine("\nProcessing area conversions...");
            batchService.ProcessAreaConversions();

            Console.WriteLine("\n=== Program Completed ===");
        }
    }
}
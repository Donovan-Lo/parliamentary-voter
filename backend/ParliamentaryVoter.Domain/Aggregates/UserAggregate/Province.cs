using System;
using System.Collections.Generic;
using System.Linq;

namespace ParliamentaryVoter.Domain.Aggregates.UserAggregate
{
    public class Province : ValueObject
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Country { get; private set; }
        public string Type { get; private set; }
        public int Population { get; private set; }
        public string Capital { get; private set; }

        private Province(string code, string name, string type, int population, string capital)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Country = "Canada";
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Population = population;
            Capital = capital ?? throw new ArgumentNullException(nameof(capital));
        }

        public static Province Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Province code cannot be null or empty", nameof(code));

            code = code.ToUpperInvariant();

            var province = CanadianProvinces.FirstOrDefault(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (province == null)
                throw new ArgumentException($"Invalid province code: {code}", nameof(code));

            return province;
        }

        public static bool IsValidCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return CanadianProvinces.Any(p => p.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyList<Province> GetAll()
        {
            return CanadianProvinces.AsReadOnly();
        }

        public static IReadOnlyList<Province> GetProvinces()
        {
            return CanadianProvinces.Where(p => p.Type == "Province").ToList().AsReadOnly();
        }

        public static IReadOnlyList<Province> GetTerritories()
        {
            return CanadianProvinces.Where(p => p.Type == "Territory").ToList().AsReadOnly();
        }

        public bool IsTerritory => Type == "Territory";

        public bool IsProvince => Type == "Province";


        /// Static list of all Canadian provinces and territories
        private static readonly List<Province> CanadianProvinces = new()
        {
            // Provinces
            new Province("AB", "Alberta", "Province", 4_428_000, "Edmonton"),
            new Province("BC", "British Columbia", "Province", 5_214_000, "Victoria"),
            new Province("MB", "Manitoba", "Province", 1_380_000, "Winnipeg"),
            new Province("NB", "New Brunswick", "Province", 789_000, "Fredericton"),
            new Province("NL", "Newfoundland and Labrador", "Province", 520_000, "St. John's"),
            new Province("NS", "Nova Scotia", "Province", 992_000, "Halifax"),
            new Province("ON", "Ontario", "Province", 15_000_000, "Toronto"),
            new Province("PE", "Prince Edward Island", "Province", 164_000, "Charlottetown"),
            new Province("QC", "Quebec", "Province", 8_575_000, "Quebec City"),
            new Province("SK", "Saskatchewan", "Province", 1_180_000, "Regina"),

            // Territories
            new Province("NT", "Northwest Territories", "Territory", 45_000, "Yellowknife"),
            new Province("NU", "Nunavut", "Territory", 40_000, "Iqaluit"),
            new Province("YT", "Yukon", "Territory", 42_000, "Whitehorse")
        };

        public override string ToString()
        {
            return $"{Name} ({Code})";
        }
    }

    /// Base class for value objects
    public abstract class ValueObject
    {
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
                return false;

            return left?.Equals(right) != false;
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject? one, ValueObject? two)
        {
            return EqualOperator(one, two);
        }

        public static bool operator !=(ValueObject? one, ValueObject? two)
        {
            return NotEqualOperator(one, two);
        }
    }
}
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LiftDataManager.Models
{
    [Serializable]
    public class TechnicalLiftDocumentation
    {
        public enum Month
        {
            Januar = 1,
            Februar,
            März,
            April,
            Mai,
            Juni,
            Juli,
            August,
            September,
            Oktober,
            November,
            Dezember
        }

        public enum ProtectedSpaceTyp
        {
            [Display(Name = "Typ1 (Aufrecht)")]
            Typ1 = 1,
            [Display(Name = "Typ2 (Hockend)")]
            Typ2 = 2,
            [Display(Name = "Typ3 (Liegend)")]
            Typ3 = 3
        }

        public TechnicalLiftDocumentation()
        {
            Years = new List<int> { 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            Months = GetMonths();
            ProtectedSpacePits = GetProtectedSpacePits();
            ProtectedSpaceHeads = GetProtectedSpaceHeads();
        }
        [JsonIgnore]
        public List<int> Years { get; set; }
        [JsonIgnore]
        public List<string> Months { get; set; }
        [JsonIgnore]
        public List<string> ProtectedSpacePits { get; set; }
        [JsonIgnore]
        public List<string> ProtectedSpaceHeads { get; set; }

        public int ManufactureYear { get; set; }
        public int YearOfConstruction { get; set; }
        public Month MonthOfConstruction { get; set; }
        public ProtectedSpaceTyp ProtectedSpaceTypPit { get; set; }
        public ProtectedSpaceTyp ProtectedSpaceTypHead { get; set; }
        public double SafetySpacePit { get; set; }
        public double SafetySpaceHead { get; set; }
        public string? SpecialFeatures { get; set; }

        private static List<string> GetMonths()
        {
            return Enum.GetNames(typeof(Month)).ToList();
        }

        private static List<string> GetProtectedSpacePits()
        {
            var protectedSpacePits = new List<string>();
            foreach (ProtectedSpaceTyp item in (ProtectedSpaceTyp[])Enum.GetValues(typeof(ProtectedSpaceTyp)))
            {
                protectedSpacePits.Add(item.Humanize());
            }
            return protectedSpacePits;
        }

        private static List<string> GetProtectedSpaceHeads()
        {
            var protectedSpaceHeads = new List<string>();
            foreach (ProtectedSpaceTyp item in (ProtectedSpaceTyp[])Enum.GetValues(typeof(ProtectedSpaceTyp)))
            {
                protectedSpaceHeads.Add(item.Humanize());
            }
            return protectedSpaceHeads.Take(2).ToList();
        }
    }
}



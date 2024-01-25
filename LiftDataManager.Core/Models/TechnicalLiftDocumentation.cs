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

        public event EventHandler? OnTechnicalLiftDocumentationChanged;

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
        public List<Month> Months { get; set; }
        [JsonIgnore]
        public List<string> ProtectedSpacePits { get; set; }
        [JsonIgnore]
        public List<string> ProtectedSpaceHeads { get; set; }

        private Month monthOfConstruction;
        public Month MonthOfConstruction
        {
            get => monthOfConstruction;
            set
            {
                monthOfConstruction = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int manufactureYear;
        public int ManufactureYear
        {
            get => manufactureYear;
            set
            {
                manufactureYear = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int yearOfConstruction;
        public int YearOfConstruction
        {
            get => yearOfConstruction;
            set
            {
                yearOfConstruction = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ProtectedSpaceTyp ProtectedSpaceTypPit { get; set; }
        public ProtectedSpaceTyp ProtectedSpaceTypHead { get; set; }

        private double safetySpacePit;
        public double SafetySpacePit
        {
            get => safetySpacePit;
            set
            {
                safetySpacePit = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private double safetySpaceHead;
        public double SafetySpaceHead
        {
            get => safetySpaceHead;
            set
            {
                safetySpaceHead = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string? specialFeatures;
        public string? SpecialFeatures
        {
            get => specialFeatures;
            set
            {
                specialFeatures = value;
                OnTechnicalLiftDocumentationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private static List<Month> GetMonths()
        {
            return Enum.GetValues(typeof(Month)).Cast<Month>().ToList();
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



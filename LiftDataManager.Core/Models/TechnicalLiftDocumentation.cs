using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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

        public event EventHandler<TechnicalLiftDocumentationEventArgs>? OnTechnicalLiftDocumentationChanged;

        public TechnicalLiftDocumentation()
        {
            Years = new List<int> { 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            Months = Enum.GetValues(typeof(Month)).Cast<Month>().ToList();
            ProtectedSpacePits = Enum.GetValues(typeof(ProtectedSpaceTyp)).Cast<ProtectedSpaceTyp>().ToList();
            ProtectedSpaceHeads = Enum.GetValues(typeof(ProtectedSpaceTyp)).Cast<ProtectedSpaceTyp>().Take(2).ToList();
        }

        [JsonIgnore]
        public List<int> Years { get; set; }
        [JsonIgnore]
        public List<Month> Months { get; set; }
        [JsonIgnore]
        public List<ProtectedSpaceTyp> ProtectedSpacePits { get; set; }
        [JsonIgnore]
        public List<ProtectedSpaceTyp> ProtectedSpaceHeads { get; set; }

        private Month monthOfConstruction;
        public Month MonthOfConstruction
        {
            get => monthOfConstruction;
            set
            {
                monthOfConstruction = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private int manufactureYear;
        public int ManufactureYear
        {
            get => manufactureYear;
            set
            {
                manufactureYear = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private int yearOfConstruction;
        public int YearOfConstruction
        {
            get => yearOfConstruction;
            set
            {
                yearOfConstruction = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private ProtectedSpaceTyp protectedSpaceTypPit;
        public ProtectedSpaceTyp ProtectedSpaceTypPit
        {
            get => protectedSpaceTypPit;
            set 
            {
                protectedSpaceTypPit = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private ProtectedSpaceTyp protectedSpaceTypHead;
        public ProtectedSpaceTyp ProtectedSpaceTypHead 
        { 
            get => protectedSpaceTypHead;
            set
            {
                protectedSpaceTypHead = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private double safetySpacePit;
        public double SafetySpacePit
        {
            get => safetySpacePit;
            set
            {
                safetySpacePit = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private double safetySpaceHead;
        public double SafetySpaceHead
        {
            get => safetySpaceHead;
            set
            {
                safetySpaceHead = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private string? specialFeatures = "---";
        public string? SpecialFeatures
        {
            get => specialFeatures;
            set
            {
                specialFeatures = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool layoutdrawing;
        public bool Layoutdrawing
        {
            get => layoutdrawing;
            set
            {
                layoutdrawing = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool riskAssessment;
        public bool RiskAssessment
        {
            get => riskAssessment;
            set
            {
                riskAssessment = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool calculations;
        public bool Calculations
        {
            get => calculations;
            set
            {
                calculations = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool circuitDiagrams;
        public bool CircuitDiagrams
        {
            get => circuitDiagrams;
            set
            {
                circuitDiagrams = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool testingInstructions;
        public bool TestingInstructions
        {
            get => testingInstructions;
            set
            {
                testingInstructions = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool factoryCertificate;
        public bool FactoryCertificate
        {
            get => factoryCertificate;
            set
            {
                factoryCertificate = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool operatingInstructions;
        public bool OperatingInstructions
        {
            get => operatingInstructions;
            set
            {
                operatingInstructions = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool maintenanceInstructions;
        public bool MaintenanceInstructions
        {
            get => maintenanceInstructions;
            set
            {
                maintenanceInstructions = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        private bool otherDocuments;
        public bool OtherDocuments
        {
            get => otherDocuments;
            set
            {
                otherDocuments = value;
                TechnicalLiftDocumentationPropertyChanged();
            }
        }

        public static string GetProtectedSpaceTypImage(ProtectedSpaceTyp protectedSpace)
        {
            return protectedSpace switch
            {
                ProtectedSpaceTyp.Typ1 => "/Images/TechnicalDocumentation/protectionRoomTyp1.png",
                ProtectedSpaceTyp.Typ2 => "/Images/TechnicalDocumentation/protectionRoomTyp2.png",
                ProtectedSpaceTyp.Typ3 => "/Images/TechnicalDocumentation/protectionRoomTyp3.png",
                _ => "/Images/NoImage.png",
            };
        }

        public static string GetProtectedSpaceTypDescription(ProtectedSpaceTyp protectedSpace)
        {
            return protectedSpace switch
            {
                ProtectedSpaceTyp.Typ1 => "Aufrecht 0,40 x 0,50 x 2,00 m",
                ProtectedSpaceTyp.Typ2 => "Hockend 0,50 x 0,70 x 1,00 m ",
                ProtectedSpaceTyp.Typ3 => "Liegend 0,70 x 1,00 x 0,50 m",
                _ => "Kein Schutzraum gewählt",
            };
        }

        private void TechnicalLiftDocumentationPropertyChanged([CallerMemberName] string propertyName = "")
        {
            OnTechnicalLiftDocumentationChanged?.Invoke(this, new TechnicalLiftDocumentationEventArgs(propertyName));
        }
    }

    public class TechnicalLiftDocumentationEventArgs : EventArgs
    {
        public TechnicalLiftDocumentationEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
    }
}



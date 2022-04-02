using System.Windows;
using System.Xml.Serialization;


namespace MissionChanger.Model
{
    public enum AircraftSourceTypeEnum
    {
        Unknown,
        Official,
        Community,
        Deluxe,
        PremiumDeluxe,
        Payware,
    }

    [XmlType(TypeName = "AircraftModel")]
    public class Aircraft : BaseModel
    {
        [XmlAttribute]
        public string Name { get => name; set => SetField(ref name, value); }
        private string name = string.Empty;

        [XmlAttribute]
        public string BaseName { get => baseName; set => SetField(ref baseName, value); }
        private string baseName = string.Empty;
        public bool ShouldSerializeBaseName() => BaseName.Length > 0;

        [XmlIgnore]
        public Thickness Margin { get => (BaseName.Length > 0 && !BaseName.StartsWith("[")) ? new Thickness(20, 2, 2, 2) : new Thickness(2); }

        [XmlAttribute]
        public string SourcePath { get => sourcePath; set => SetField(ref sourcePath, value); }
        private string sourcePath = string.Empty;
        public bool ShouldSerializeSourcePath() => SourcePath.Length > 0;

        [XmlAttribute]
        public string AltSourcePath { get => altSourcePath; set => SetField(ref altSourcePath, value); }
        private string altSourcePath = string.Empty;
        public bool ShouldSerializeAltSourcePath() => altSourcePath.Length > 0;

        [XmlAttribute]
        public AircraftSourceTypeEnum SourceType { get => sourceType; set => SetField(ref sourceType, value); }
        private AircraftSourceTypeEnum sourceType = AircraftSourceTypeEnum.Unknown;


        public override string ToString()
        {
            return Name;
            // return string.Format("{0}, Vr={1} ", Name, CruiseSpeed);
        }
    }
}

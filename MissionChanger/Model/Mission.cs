using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionChanger.Model
{
    public class Mission : BaseModel
    {
        public string Name { get => name; set => SetField(ref name, value); }
        private string name;

        public string MissionType { get => missionType; set => SetField(ref missionType, value); }
        private string missionType;

        public string Aircraft { get => aircraft; set => SetField(ref aircraft, value); }
        private string aircraft;

        public string OriginalAircraft { get => originalAircraft; set => SetField(ref originalAircraft, value); }
        private string originalAircraft = String.Empty;
        
        public string Filename { get => filename; set => SetField(ref filename, value); }
        private string filename;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionChanger.Model
{
    public class Aircraft : BaseModel
    {
        private string name = string.Empty;
        private int cruiseSpeed = 0;

        public string Name { get => name; set => SetField(ref name, value); }
        public int CruiseSpeed { get => cruiseSpeed; set => SetField(ref cruiseSpeed, value); }

        public override string ToString()
        {
            return Name;
            // return string.Format("{0}, Vr={1} ", Name, CruiseSpeed);
        }
    }
}

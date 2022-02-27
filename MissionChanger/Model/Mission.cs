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
        public string Title { get => title; set => SetField(ref title, value); }
        private string title;

        public string MissionType { get => missionType; set => SetField(ref missionType, value); }
        private string missionType;

        public string Aircraft { get => aircraft; set => SetField(ref aircraft, value); }
        private string aircraft;

        public string OriginalAircraft { get => originalAircraft; set => SetField(ref originalAircraft, value); }
        private string originalAircraft = String.Empty;
        
        public string Filename { get => filename; set => SetField(ref filename, value); }
        private string filename;

        public DateTime DateTime { get => dateTime; set => SetField(ref dateTime, value); }
        private DateTime dateTime;

        public DateTime DateOnly
        {
            get => dateTime.Date;
            set
            {
                if (DateTime.Date.CompareTo(value) != 0)
                {
                    DateTime dt = new DateTime(value.Year, value.Month, value.Day, Hour, Minute, Second);
                    SetField(ref dateTime, dt, nameof(DateTime));
                }
            }
        }

        public int Year { get => DateTime.Year; set => SetField(ref dateTime, new DateTime(value, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, DateTime.Second)); }
        public int Month { get => DateTime.Month; set => SetField(ref dateTime, new DateTime(DateTime.Year, value, DateTime.Day, DateTime.Hour, DateTime.Minute, DateTime.Second)); }
        public int Day { get => DateTime.Day; set => SetField(ref dateTime, new DateTime(DateTime.Year, DateTime.Month, value, DateTime.Hour, DateTime.Minute, DateTime.Second)); }
        public int Hour { get => DateTime.Hour; set => SetField(ref dateTime, new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, value, DateTime.Minute, DateTime.Second)); }
        public int Minute { get => DateTime.Minute; set => SetField(ref dateTime, new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, value, DateTime.Second)); }
        public int Second { get => DateTime.Second; set => SetField(ref dateTime, new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, value)); }

        public int DayOfYear { get => DateTime.DayOfYear; }

        public bool UseWeatherFile { get => useWeatherFile; set => SetField(ref useWeatherFile, value); }
        private bool useWeatherFile = false;

        public bool UseLiveWeather { get => useLiveWeather; set => SetField(ref useLiveWeather, value); }
        private bool useLiveWeather = false;

        public string WeatherPresetFile { get => weatherPresetFile; set => SetField(ref weatherPresetFile, value); }
        private string weatherPresetFile = string.Empty;

        public bool WeatherCanBeLive { get => weatherCanBeLive; set => SetField(ref weatherCanBeLive, value); }
        private bool weatherCanBeLive = false;

        public bool LostGPS { get => lostGPS; set => SetField(ref lostGPS, value); }
        private bool lostGPS = false;

        public bool IsChanged { get => isChanged; set => SetField(ref isChanged, value); }
        private bool isChanged = false;

        public bool IsProtected { get => isProtected; set => SetField(ref isProtected, value); }
        private bool isProtected = false;

        public bool HasWeatherFile { get => hasWeatherFile; set => SetField(ref hasWeatherFile, value); }
        private bool hasWeatherFile = false;

        public bool HasBackup { get => hasBackup; set => SetField(ref hasBackup, value); }
        private bool hasBackup = false;

        public Manifest Manifest { get => manifest; set => SetField(ref manifest, value); }
        private Manifest manifest = null;

        public List<Mission> SavedMissions { get => savedMissions; set => SetField(ref savedMissions, value); }
        private List<Mission> savedMissions = new List<Mission>();

        
   }
}

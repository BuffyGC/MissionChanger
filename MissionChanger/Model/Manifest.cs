using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionChanger.Model
{
    public class Manifest
    {
        public Dependency[] dependencies { get; set; }
        public string content_type { get; set; }
        public string title { get; set; }
        public string manufacturer { get; set; }
        public string creator { get; set; }
        public string package_version { get; set; }
        public string minimum_game_version { get; set; }
        public Release_Notes release_notes { get; set; }
        public string total_package_size { get; set; }
    }

    public class Release_Notes
    {
        public Neutral neutral { get; set; }
    }

    public class Neutral
    {
        public string LastUpdate { get; set; }
        public string OlderHistory { get; set; }
    }

    public class Dependency
    {
        public string name { get; set; }
        public string package_version { get; set; }
    }
}

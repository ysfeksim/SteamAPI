using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SteamAPI
{
    public class GameData
    {
        [XmlElement("Game")]
        public SteamWebAPI.SteamAPISession.Game _game { get; set; }
        //[XmlArray("Achievements")]
        //[XmlArrayItem("Achievement")]
        //public List<SteamWebAPI.SteamAPISession.Achievement> _achievementList { get; set; }
        public GameData()
        {
            _game = new SteamWebAPI.SteamAPISession.Game();
            //_achievementList = new List<SteamWebAPI.SteamAPISession.Achievement>();
        }
    }
}

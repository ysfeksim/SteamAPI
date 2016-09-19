using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SteamAPI
{
    [XmlRoot("UserList")]
    public class UserData
    {
        [XmlElement("User")]
        public SteamWebAPI.SteamAPISession.User _user { get; set; }
        [XmlArray("Games")]
        [XmlArrayItem("Game")]
        public List<GameData> _gameList { get; set; }
        public UserData()
        {
            _user = new SteamWebAPI.SteamAPISession.User();
            _gameList = new List<GameData>();
        }
    }
}

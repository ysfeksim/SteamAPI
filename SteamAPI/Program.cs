using SteamWebAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{
    class Program
    {
        static void Main()
        {
            SteamAPIv2 api = new SteamAPIv2();
            List<string> steamids = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                string s = ConfigurationManager.AppSettings["steamid"].ToString();
                steamids.Add(s);
            }
            //api.GetPlayerSummaries(steamids);

            List<SteamAPISession.User> userList = new List<SteamAPISession.User>();
            userList = api.GetPlayerSummaries(steamids);
            //userList = api.GetFriendList(ConfigurationManager.AppSettings["steamid"].ToString());
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    try
                    {
                        userList.AddRange(api.GetFriendList(userList[i].steamid));
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
            userList = (from x in userList
                        where x.profileVisibility == SteamWebAPI.SteamAPISession.ProfileVisibility.Public
                        group x by new { x.steamid }
                            into mygroup
                            select mygroup.FirstOrDefault()).ToList();
            List<UserData> ul = new List<UserData>();
            List<SteamAPISession.Game> gl = new List<SteamAPISession.Game>();
            foreach (var user in userList)
            {
                UserData u = new UserData();
                u._user = user;
                var gamelist = api.GetOwnedGames(user.steamid);
                //if (gamelist != null)
                //{
                //    foreach (var game in gamelist)
                //    {
                //        GameData g = new GameData();
                //        g._game = game;
                //        g._game.AchievementList = api.GetPlayerAchievements(user.steamid, game.gameid);
                //        if (g._game.AchievementList != null && g._game.AchievementList.Count > 0)
                //        {
                //            g._game.gamename = g._game.AchievementList.First().gamename;
                //        }
                //        u._gameList.Add(g);
                //    }
                //}
                gl.AddRange(gamelist);
                ul.Add(u);
            }
            DataSave.WriteXML(ul, typeof(UserData));
            DataSave.WriteXML(gl, typeof(SteamAPISession.Game));
            //SteamWebAPI.SteamAPISession.LoginStatus s = api.Authenticate(ConfigurationManager.AppSettings["steamid"]);
            Console.WriteLine(ConfigurationManager.AppSettings["steamid"]);
            Console.WriteLine(ConfigurationManager.AppSettings["steamid"]);

        }


    }
}

using HtmlAgilityPack;
using SteamWebAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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
            for (int i = 0; i < 3; i++)
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
                u._user.totalPlayTime = 0;
                var gamelist = api.GetOwnedGames(user.steamid, user.include_appinfo);
                if (gamelist != null)
                {
                    foreach (var item in gamelist)
                    {
                        GameData g = new GameData();
                        g._game = item;
                        u._user.totalPlayTime += Convert.ToInt32(item.playtime_forever);
                        u._gameList.Add(g);
                    }
                }
                /*if (gamelist != null)
                {
                    foreach (var game in gamelist)
                    {
                        GameData g = new GameData();
                        g._game = game;
                        g._game.AchievementList = api.GetPlayerAchievements(user.steamid, game.gameid);
                        if (g._game.AchievementList != null && g._game.AchievementList.Count > 0)
                        {
                            g._game.gamename = g._game.AchievementList.First().gamename;
                        }
                        u._gameList.Add(g);
                    }
                }*/
                ul.Add(u);
                if (gamelist != null)
                {
                    gl.AddRange(gamelist);
                }

            }

            gl = (from x in gl
                  group x by new { x.gamename }
                      into mygroup
                      select mygroup.FirstOrDefault()).ToList();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + "Gamelist" + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(path))
            {

                foreach (var line in gl)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(line.gameid + "|" + line.gamename);

                }

            }

            var pathuser = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + "Userlist" + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(pathuser))
            {

                foreach (var lineuser in ul)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file.
                    file.WriteLine(lineuser._user.steamid + "|" + lineuser._user.nickname + "|" + lineuser._user.profileUrl);

                }

            }

            var pathusergame = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + "UserGameRating" + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(pathusergame))
            {

                foreach (var lineuser in ul)
                {
                    foreach (var item in lineuser._gameList)
                    {
                        double rating = lineuser._user.totalPlayTime != 0 ? Convert.ToDouble(item._game.playtime_forever) / Convert.ToDouble(lineuser._user.totalPlayTime) : 0;
                        rating *= 10;
                        rating = Math.Round(rating, 1);
                        if (rating != 0)
                        {
                            file.WriteLine(lineuser._user.steamid + "|" + item._game.gameid + "|" + rating.ToString());
                        }

                    }
                }

            }

            var pathusergame3 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + "GameTag" + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(pathusergame3))
            {
                foreach (var item2 in gl)
                {
                    string link = "http://store.steampowered.com/app/" + item2.gameid;

                    using (var client = new WebClient())
                    {
                        var htmlSource = client.DownloadString(link);
                        var doc = new HtmlDocument();
                        doc.LoadHtml(htmlSource);
                        var aList = doc.DocumentNode.SelectNodes("//a").ToList();
                        List<HtmlNode> tags = new List<HtmlNode>();
                        foreach (var item in aList)
                        {
                            try
                            {
                                if (item.Attributes["class"].Value == "app_tag")
                                {
                                    tags.Add(item);
                                    if (tags.Count >= 10)
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }

                        }
                        int index = 10;
                        foreach (var item in tags)
                        {
                            file.WriteLine(item2.gameid + "|" + item.InnerText.Replace("\t", "").Replace("\r", "") + "|"+index);
                            index--;
                        }
                    }
                }
            }



            DataSave.WriteXML(ul, typeof(UserData));
            DataSave.WriteXML(gl, typeof(SteamAPISession.Game));
            //SteamWebAPI.SteamAPISession.LoginStatus s = api.Authenticate(ConfigurationManager.AppSettings["steamid"]);
            Console.WriteLine(ConfigurationManager.AppSettings["steamid"]);
            Console.WriteLine(ConfigurationManager.AppSettings["steamid"]);

        }


    }
}

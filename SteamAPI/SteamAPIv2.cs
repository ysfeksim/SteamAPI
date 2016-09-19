using Newtonsoft.Json.Linq;
using SteamWebAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{
    class SteamAPIv2
    {
        /// <summary>
        ///http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=XXXXXXXXXXXXXXXXXXXXXXX&steamids=76561197960435530
        /// </summary>
        /// <param name="steamid"></param>
        /// <returns></returns>
        public List<SteamAPISession.User> GetPlayerSummaries(List<string> steamids)
        {
            string response = steamRequest("ISteamUser/GetPlayerSummaries/v0002/?key=" + ConfigurationManager.AppSettings["steamkey"] + "&steamids=" + String.Join(",", steamids.GetRange(0, Math.Min(steamids.Count, 100)).ToArray()));
            if (response != null)
            {
                JObject parsed = JObject.Parse(response);
                //DataSave.toXml(parsed.ToString(), "PlayerSummaries");
                JObject data = (JObject)parsed["response"];
                if (data["players"] != null)
                {
                    List<SteamWebAPI.SteamAPISession.User> users = new List<SteamWebAPI.SteamAPISession.User>();

                    foreach (JObject info in data["players"])
                    {
                        SteamWebAPI.SteamAPISession.User user = new SteamWebAPI.SteamAPISession.User();

                        user.steamid = (String)info["steamid"];
                        user.profileVisibility = (SteamWebAPI.SteamAPISession.ProfileVisibility)(int)info["communityvisibilitystate"];
                        user.nickname = (String)info["personaname"];
                        try
                        {
                            user.lastLogoff = unixTimestamp((long)info["lastlogoff"]);
                            user.profileUrl = (String)info["profileurl"];
                            user.status = (SteamWebAPI.SteamAPISession.UserStatus)(int)info["personastate"];
                            user.avatarUrl = info["avatar"] != null ? (String)info["avatar"] : "";
                            if (user.avatarUrl != null) user.avatarUrl = user.avatarUrl.Substring(0, user.avatarUrl.Length - 4);
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            user.profileState = (int)info["profilestate"];
                            user.joinDate = unixTimestamp(info["timecreated"] != null ? (long)info["timecreated"] : 0);
                            user.primaryGroupId = info["primaryclanid"] != null ? (String)info["primaryclanid"] : "";
                            user.realName = info["realname"] != null ? (String)info["realname"] : "";
                            user.locationCountryCode = info["loccountrycode"] != null ? (String)info["loccountrycode"] : "";
                            user.locationStateCode = info["locstatecode"] != null ? (String)info["locstatecode"] : "";
                            user.locationCityId = info["loccityid"] != null ? (int)info["loccityid"] : -1;

                        }
                        catch (Exception)
                        {
                        }

                        users.Add(user);
                    }

                    // Requests are limited to 100 steamids, so issue multiple requests
                    if (steamids.Count > 100)
                        users.AddRange(GetPlayerSummaries(steamids.GetRange(100, Math.Min(steamids.Count - 100, 100))));

                    return users;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// http://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX&steamid=76561197960435530&relationship=friend
        /// </summary>
        /// <param name="steamid"></param>
        /// <returns></returns>
        public List<SteamAPISession.User> GetFriendList(string steamid)
        {
            string response = steamRequest("ISteamUser/GetFriendList/v0001/?key=" + ConfigurationManager.AppSettings["steamkey"] + "&steamid=" + steamid + "&relationship=friend");
            if (response != null)
            {
                JObject parsed = JObject.Parse(response);
                JObject data = (JObject)parsed["friendslist"];
                List<SteamAPISession.User> friend_list = new List<SteamAPISession.User>();
                List<string> sl = new List<string>();
                foreach (JObject item in data["friends"])
                {
                    sl.Add(item["steamid"].ToString());
                }
                friend_list.AddRange(GetPlayerSummaries(sl));
                return friend_list;
            }
            return null;
        }

        /// <summary>
        ///  http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v0001/?appid=440&key=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX&steamid=76561197972495328
        /// </summary>
        /// <param name="steamids"></param>
        /// <returns></returns>
        public List<SteamAPISession.Achievement> GetPlayerAchievements(string steamid, string appid)
        {
            string response = steamRequest("ISteamUserStats/GetPlayerAchievements/v0001/?appid=" + appid + "&key=" + ConfigurationManager.AppSettings["steamkey"] + "&steamid=" + steamid);
            if (response != null)
            {
                JObject parsed = JObject.Parse(response);
                JObject data = (JObject)parsed["playerstats"];
                if (data["achievements"] != null)
                {
                    List<SteamWebAPI.SteamAPISession.Achievement> achievements = new List<SteamWebAPI.SteamAPISession.Achievement>();

                    foreach (JObject info in data["achievements"])
                    {
                        SteamWebAPI.SteamAPISession.Achievement achievement = new SteamWebAPI.SteamAPISession.Achievement();
                        achievement.gamename = (string)data["gameName"];
                        achievement.apiname = (string)info["apiname"];
                        achievement.achieved = (string)info["achieved"];
                        achievements.Add(achievement);
                    }

                    // Requests are limited to 100 steamids, so issue multiple requests
                    //if (steamids.Count > 100)
                    //    users.AddRange(GetUserInfo(steamids.GetRange(100, Math.Min(steamids.Count - 100, 100))));

                    return achievements;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return new List<SteamAPISession.Achievement>();
            }

        }

        /// <summary>
        /// http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=XXXXXXXXXXXXXXXXX&steamid=76561197960434622&format=json
        /// </summary>
        /// <param name="steamid"></param>
        /// <returns></returns>
        public List<SteamAPISession.Game> GetOwnedGames(string steamid)
        {
            string response = steamRequest("IPlayerService/GetOwnedGames/v0001/?key=" + ConfigurationManager.AppSettings["steamkey"] + "&steamid=" + steamid + "&format=json");
            if (response != null)
            {
                JObject parsed = JObject.Parse(response);
                JObject data = (JObject)parsed["response"];
                if (data["games"] != null)
                {
                    List<SteamWebAPI.SteamAPISession.Game> games = new List<SteamWebAPI.SteamAPISession.Game>();

                    foreach (JObject info in data["games"])
                    {
                        SteamWebAPI.SteamAPISession.Game game = new SteamWebAPI.SteamAPISession.Game();

                        game.gameid = (string)info["appid"];
                        game.playtime_forever = (string)info["playtime_forever"];
                        games.Add(game);
                    }

                    // Requests are limited to 100 steamids, so issue multiple requests
                    //if (steamids.Count > 100)
                    //    users.AddRange(GetUserInfo(steamids.GetRange(100, Math.Min(steamids.Count - 100, 100))));

                    return games;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// Change the datetime format
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private DateTime unixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        /// <summary>
        /// Webservice connection
        /// </summary>
        /// <param name="get"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        private String steamRequest(String get)
        {
            System.Net.ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.steampowered.com/" + get);
            //request.Host = "api.steampowered.com:443";
            //request.ProtocolVersion = HttpVersion.Version11;
            //request.Accept = "*/*";
            //request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            //request.Headers[HttpRequestHeader.AcceptLanguage] = "en-us";
            //request.UserAgent = "Steam 1291812 / iPhone";

            //if (post != null)
            //{
            //    request.Method = "POST";
            //    byte[] postBytes = Encoding.ASCII.GetBytes(post);
            //    request.ContentType = "application/x-www-form-urlencoded";
            //    request.ContentLength = postBytes.Length;

            //    Stream requestStream = request.GetRequestStream();
            //    requestStream.Write(postBytes, 0, postBytes.Length);
            //    requestStream.Close();
            //}

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if ((int)response.StatusCode != 200)
                {
                    Console.WriteLine(response.GetResponseStream());
                }
                String src = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                return src;
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}

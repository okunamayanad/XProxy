﻿using System.Text.RegularExpressions;

namespace XProxy.Core
{
    public class PlaceHolders
    {
        public static string ReplacePlaceholders(string text)
        {
            return Regex.Replace(text, @"%(\w+?)(?:_(\w+))?%", match =>
            {
                string placeholderType = match.Groups[1].Value.ToLower();
                string serverName = match.Groups[2].Success ? match.Groups[2].Value : null;

                Server.TryGetByName(serverName, out Server targetServer);

                switch (placeholderType.ToLower())
                {
                    case "playersinqueue":
                        if (targetServer == null)
                            return "-1";

                        return $"{targetServer.PlayersInQueueCount}";
                    case "onlineplayers":
                        if (targetServer == null)
                            return "-1";

                        return $"{targetServer.PlayersCount}";
                    case "maxplayers":
                        if (targetServer == null)
                            return "-1";

                        return $"{targetServer.PlayersCount}";
                    case "proxyonlineplayers":
                        return Player.Count.ToString(); 
                    case "proxymaxplayers":
                        if (!Listener.TryGet(serverName, out Listener list))
                            return "-1";

                        return list.Settings.MaxPlayers.ToString();
                    default:
                        return "%placeholder_not_found%";
                }
            });
        }
    }
}

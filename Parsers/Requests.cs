using System.Net;
using System.Text;
using Nure.NET.Types;

namespace Nure.NET.Parsers;

public class Requests
{
    private static readonly string[] Servers = { "cist.nure.ua", "cist2.nure.ua" };

    private static string GetAvailableServer()
    {
        var ping = new System.Net.NetworkInformation.Ping();

        foreach (var server in Servers)
        {
            var result = ping.Send(server, 500);
            if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                // Додаткова перевірка HTTP
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync($"https://{server}").Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return server;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        return string.Empty;
    }

    private static string FetchJsonFromUrl(string endpoint)
    {
        var server = GetAvailableServer();
        if (string.IsNullOrEmpty(server))
        {
            return "";
        }

        var fullUrl = $"https://{server}{endpoint}";

        try
        {
            var webRequest = WebRequest.Create(fullUrl) as HttpWebRequest;
            webRequest.ContentType = "application/json";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var webResponse = webRequest.GetResponse())
            using (var streamReader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                streamWriter.Write(streamReader.ReadToEnd());
                streamWriter.Flush();
                memoryStream.Position = 0;

                var json = Encoding.UTF8.GetString(memoryStream.ToArray());
                json = json.TrimStart('\uFEFF');
                return json;
            }
        }
        catch
        {
            return "";
        }
    }

    public static string GetAuditoriesJson()
    {
        return FetchJsonFromUrl("/ias/app/tt/P_API_AUDITORIES_JSON");
    }

    public static string GetGroupsJson()
    {
        return FetchJsonFromUrl("/ias/app/tt/P_API_GROUP_JSON");
    }

    public static string GetTeachersJson()
    {
        var json = FetchJsonFromUrl("/ias/app/tt/P_API_PODR_JSON");
        if (!string.IsNullOrEmpty(json))
        {
            json = json.Remove(json.Length - 2);
            json += "]}}";
        }
        return json;
    }

    private static long GetUnixTimestamp(DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeSeconds();
    }

    private static long StartTime()
    {
        var currentYear = DateTime.Now.Year;
        var targetYear = currentYear - 2;
        var july1 = new DateTime(targetYear, 7, 1);
        return GetUnixTimestamp(july1);
    }

    private static long EndTime()
    {
        var currentYear = DateTime.Now.Year;
        var targetYear = currentYear + 2;
        var september1 = new DateTime(targetYear, 9, 1);
        return GetUnixTimestamp(september1);
    }

    public static string GetEventsJson(EventType type, long id)
    {
        var url = $"/ias/app/tt/P_API_EVEN_JSON?" +
                  $"type_id={(int)type}" +
                  $"&timetable_id={id}" +
                  $"&time_from={StartTime()}" +
                  $"&time_to={EndTime()}" +
                  "&idClient=KNURESked";

        return FetchJsonFromUrl(url);
    }
}

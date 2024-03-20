using System.Net;
using System.Text;
using Nure.NET.Types;

namespace Nure.NET.Parsers;

public class Requests
{
    public static string GetAuditoriesJson()
    {
        var ping = new System.Net.NetworkInformation.Ping();

        var result = ping.Send("cist.nure.ua", 300);

        if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
        {
            return "";
        }
        else
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var webRequest = WebRequest.Create("https://cist.nure.ua/ias/app/tt/P_API_AUDITORIES_JSON") as HttpWebRequest;

                webRequest.ContentType = "application/json";

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var webResponse = webRequest.GetResponse())
                using (var streamReader =
                       new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                    streamWriter.Flush();
                    memoryStream.Position = 0;

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());

                    // Remove BOM
                    json = json.TrimStart('\uFEFF');

                    return json;
                }
            }
        }
    }

    public static string GetGroupsJson()
    {
        var ping = new System.Net.NetworkInformation.Ping();

        var result = ping.Send("cist.nure.ua", 300);

        if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
        {
            return "";
        }
        else
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var webRequest =
                    WebRequest.Create("https://cist.nure.ua/ias/app/tt/P_API_GROUP_JSON") as HttpWebRequest;

                webRequest.ContentType = "application/json";

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var webResponse = webRequest.GetResponse())
                using (var streamReader =
                       new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                    streamWriter.Flush();
                    memoryStream.Position = 0;

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());

                    // Remove BOM
                    json = json.TrimStart('\uFEFF');

                    return json;
                }
            }
        }
    }

    public static string GetTeachersJson()
    {
        var ping = new System.Net.NetworkInformation.Ping();

        var result = ping.Send("cist.nure.ua", 300);

        if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
        {
            return "";
        }
        else
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var webRequest = WebRequest.Create("https://cist.nure.ua/ias/app/tt/P_API_PODR_JSON") as HttpWebRequest;

                webRequest.ContentType = "application/json";

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var webResponse = webRequest.GetResponse())
                using (var streamReader =
                       new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                    streamWriter.Flush();
                    memoryStream.Position = 0;

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());

                    // Remove BOM
                    json = json.TrimStart('\uFEFF');

                    return json;
                }
            }
        }
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
        var ping = new System.Net.NetworkInformation.Ping();

        var result = ping.Send("cist.nure.ua", 300);

        if (result.Status != System.Net.NetworkInformation.IPStatus.Success)
        {
            return "";
        }
        else
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var webRequest = WebRequest.Create($"https://cist.nure.ua/ias/app/tt/P_API_EVEN_JSON?" +
                                                   $"type_id={(int)type}" +
                                                   $"&timetable_id={id}" +
                                                   $"&time_from={StartTime()}" +
                                                   $"&time_to={EndTime()}" +
                                                   "&idClient=KNURESked") as HttpWebRequest;

                webRequest.ContentType = "application/json";

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var webResponse = webRequest.GetResponse())
                using (var streamReader =
                       new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                using (var memoryStream = new MemoryStream())
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    streamWriter.Write(streamReader.ReadToEnd());
                    streamWriter.Flush();
                    memoryStream.Position = 0;

                    var json = Encoding.UTF8.GetString(memoryStream.ToArray());

                    // Remove BOM
                    json = json.TrimStart('\uFEFF');

                    return json;
                }
            }
        }
    }
}
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.IO;

namespace BreadDelivery
{
        internal class Program
        {
            private const string RequestUrl = @"https://api-maps.yandex.ru/services/route/2.0/?callback=id_146124731114152263626&lang=ru_RU&token=6d3545d7aa6bfbf98d6a5038715e3dbd&rll=43.981953%2C56.304944~43.9847%2C56.291964&rtm=dtr&results=3";

            private static string Parse(Stream response)
            {
                using (var xr = new StreamReader(response))
                {
                    //xr.MoveToContent();
                    //xr.Read();
                    //xr.Read();
                    //if (!xr.ReadToDescendant("AdministrativeAreaName"))
                    //{
                    //    return "Адрес не найден";
                    //}
                    return xr.ReadToEnd();
                    //var admName = xr.ReadElementContentAsString();
                    //if (!admName.EndsWith("область"))
                    //{
                    //    xr.ReadToDescendant("SubAdministrativeAreaName");
                    //    return xr.ReadElementContentAsString();
                    //}
                }
            }

            private static string GetRoute(/*string address*/)
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Concat(RequestUrl/*, address*/));
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return Parse(responseStream);
                    }
                }
            }

            static void Main(string[] args)
            {
                File.WriteAllText("response.xml", GetRoute());
                //var appender = File.AppendText("with_regions.csv");
                ////Console.WriteLine(Parse(new FileStream("ex.xml", FileMode.Open)));
                //using (var sr = new StreamReader("adres.csv"))
                //{
                //    string query;

                //    while (!sr.EndOfStream)
                //    {
                //        query = sr.ReadLine();
                //        appender.Write(query);
                //        appender.Write(';');
                //        appender.WriteLine(GetRegion(query));
                //        appender.Flush();
                //    }
                //}
                //appender.Close();
            }
        }
}

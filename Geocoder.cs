using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace BreadDelivery
{
    static class Geocoder
    {
        private const string RequestUrl = @"https://geocode-maps.yandex.ru/1.x/?geocode=";

        private static string Parse(Stream response)
        {
            using (var xr = XmlReader.Create(response))
            {
                xr.MoveToContent();
                xr.Read();
                xr.Read();
                if (!xr.ReadToDescendant("AdministrativeAreaName"))
                {
                    return "Адрес не найден";
                }
                return xr.ReadElementContentAsString();
                //var admName = xr.ReadElementContentAsString();
                //if (!admName.EndsWith("область"))
                //{
                //    xr.ReadToDescendant("SubAdministrativeAreaName");
                //    return xr.ReadElementContentAsString();
                //}
            }
        }

        private static string GetRegion(string address)
        {
            var request = (HttpWebRequest)WebRequest.Create(String.Concat(RequestUrl, address));
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    return Parse(responseStream);
                }
            }
        }

        private static string ParsePos(Stream response)
        {
            using (var xr = XmlReader.Create(response))
            {
                xr.MoveToContent();
                xr.ReadToFollowing("featureMember");
                return xr.ReadToDescendant("pos") ? xr.ReadElementContentAsString() : "Адрес не найден";
            }
        }

        private static string GetPos(string address)
        {
            var request = (HttpWebRequest)WebRequest.Create(String.Concat(RequestUrl, address));
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    return ParsePos(responseStream);
                }
            }
        }

        public static void PrintCoord()
        {
            //using (var sr = new StreamReader("shops.csv", Encoding.Default))
            //{
            //    string query;
            //    char[] sep = {';'};
            //    sr.ReadLine();
            //    while (!sr.EndOfStream)
            //    {
            //        query = sr.ReadLine().Split(sep)[1];
            //        //appender.Write(query);
            //        //appender.Write(';');
            //        Console.Write(query + ": ");
            //        Console.WriteLine(GetPos("Нижегородская область, " + query));
            //        Console.ReadKey();
            //        //appender.Flush();
            //    }
            //}

            var appender = File.AppendText("with_coords.csv");
            ////Console.WriteLine(Parse(new FileStream("ex.xml", FileMode.Open)));
            using (var sr = new StreamReader("shops.csv", Encoding.Default))
            {
                string query;
                char[] sep = { ';' };
                sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    query = sr.ReadLine().Split(sep)[1];
                    appender.Write(query);
                    appender.Write(';');
                    appender.WriteLine(GetPos("Нижегородская область, " + query));
                    appender.Flush();
                }
            }
            appender.Close();
        }
    }
}

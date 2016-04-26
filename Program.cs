using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Firefox;

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
            var driver = new FirefoxDriver();//FirefoxDriver();//new PhantomJSDriver(new PhantomJSOptions());
            driver.Navigate().GoToUrl("https://yandex.ru/maps/47/nizhny-novgorod/");
            //driver.Manage().Window.Size = new System.Drawing.Size(1024, 800);
            
            
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("toggle-button_islet-air__icon")));
            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("toggle-button_islet-air__icon")));
            var routesButton = driver.FindElementByClassName("toggle-button_islet-air__text");
            routesButton.Click();
            Thread.Sleep(5 * 1000);

            var pointFields = driver.FindElementsByXPath("//input[@placeholder=\"Адрес или точка на карте\"]").ToArray();
            if (pointFields.Length < 1)
            {
                Console.WriteLine("FAIL");
                driver.GetScreenshot().SaveAsFile("lastView.png", System.Drawing.Imaging.ImageFormat.Png);
                driver.Quit();
                return;
            }
                
            pointFields[0].SendKeys("Горная, 6а");
            pointFields[1].SendKeys("Жукова, 20\n");
            //driver.FindElementById("uniqc1").SendKeys(Keys.Return);
            Thread.Sleep(5 * 1000);
            
            var jams = driver.FindElementByClassName("checkbox_islet__control");
            jams.Click();
            
            var routeInfo = driver.FindElementByClassName("route-view_driving__route-title-text");
            Console.WriteLine("INFO: " + routeInfo.Text);
            driver.GetScreenshot().SaveAsFile("route.png", System.Drawing.Imaging.ImageFormat.Png);
            //File.WriteAllText("response.xml", GetRoute());
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
            driver.Quit();
        }
        }
}

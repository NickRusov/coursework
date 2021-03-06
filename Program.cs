﻿using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System;
using System.Threading;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Firefox;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace BreadDelivery
{
    internal class Program
    {
        private static List<Customer> ReadCsvWithCustomers(string fileName)
        {
            List<Customer> customers = new List<Customer>(550);
            Customer currentCustomer = new Customer();
            int order = 0;
            using (var sr = new StreamReader(fileName, Encoding.Default))
            {
                string[] rawCustomer;
                char[] sep = { ';' };
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";

                while (!sr.EndOfStream)
                {
                    rawCustomer = sr.ReadLine().Split(sep);
                    currentCustomer.Id = ++order;
                    currentCustomer.Address = rawCustomer[0];
                    currentCustomer.hasEuroLot = (rawCustomer[1] == "Евро");
                    //Console.WriteLine(rawCustomer[2]);
                    currentCustomer.Demand = float.Parse(rawCustomer[2], System.Globalization.NumberStyles.Any, ci);
                    //Console.WriteLine(rawCustomer[3]);
                    currentCustomer.Latitude = float.Parse(rawCustomer[3], System.Globalization.NumberStyles.Any, ci);
                    //Console.WriteLine(rawCustomer[4]);
                    currentCustomer.Longitude = float.Parse(rawCustomer[4], System.Globalization.NumberStyles.Any, ci);
                    customers.Add(currentCustomer);
                }
                
            }
            
            return customers;
        }

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

    //static void Main(string[] args)
    //{
    //    Geocoder.PrintCoord();
    //}
        static void Main(string[] args)
        {
            var customers = ReadCsvWithCustomers(@"C:\Users\user\Documents\coursework\shops_filtered.csv");
            //Console.ReadKey();

            var driver = new FirefoxDriver();//FirefoxDriver();//new PhantomJSDriver(new PhantomJSOptions());
            driver.Navigate().GoToUrl("https://yandex.ru/maps/47/nizhny-novgorod/");
            Console.WriteLine("DONE!");
            //driver.Manage().Window.Size = new System.Drawing.Size(1024, 800);


            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
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


            
            // cycle body
            pointFields[0].SendKeys("56.317892 43.98793");
            pointFields[1].SendKeys("56.320473 43.998288\n");

            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("checkbox_islet__tick")));
            var jams = driver.FindElementByClassName("checkbox_islet__tick");
            jams.Click();


            wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("route-view_driving__route-title")));
            var routeInfo = driver.FindElementByClassName("route-view_driving__route-title");
            Console.WriteLine("INFO: " + routeInfo.Text);
            Console.WriteLine(driver.FindElementByClassName("route-view_driving__route-subtitle").Text);// route-view_driving__route-subtitle

            var appender = File.AppendText("routes.csv");
            for (int i = 0; i < customers.Count; i++)
            {
                pointFields[0].SendKeys(customers[i].CoordString);
                appender.Write(i.ToString() + ": ");
                for (int j = 0; j < customers.Count; j++)
                {
                    if (i != j)
                    {
                        pointFields[1].SendKeys(customers[j].CoordString + "\n");
                        wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("route-view_driving__route-title")));
                        routeInfo = driver.FindElementByClassName("route-view_driving__route-title");
                        appender.Write(routeInfo.Text + ";");
                    }
                    else
                    {
                        appender.Write("0 , 0 ;");
                    }
                    appender.Flush();
                }
                appender.WriteLine();
            }
            appender.Close();

            //driver.GetScreenshot().SaveAsFile("route.png", System.Drawing.Imaging.ImageFormat.Png);
            //File.WriteAllText("response.xml", GetRoute());
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

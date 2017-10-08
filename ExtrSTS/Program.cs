using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;


namespace ParseBK
{
    public class Program
    {
        public static IWebDriver Driver { get; set; }

        static void Main(string[] args)
        {
            //var client = new WebClient();
            //var text = client.DownloadString("https://www.sts.pl/pl/oferta/zaklady-bukmacherskie/");
            //Console.WriteLine(text);

            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl("https://www.sts.pl/pl/oferta/zaklady-bukmacherskie/");

            // === wyodrębnienie i kliknięcie linku piłka nożna
            IWebElement LinkPilkaNozna = Driver.FindElement(By.LinkText("Piłka Nożna"));
            LinkPilkaNozna.Click();
            System.Threading.Thread.Sleep(2000);

            // === wyodrębienie wierszy z tabeli Piłki Noznej
            IWebElement TabelaPilkaNozna = Driver.FindElement(By.Id("sport_184"));
            IList<IWebElement> ListaRegionow = TabelaPilkaNozna.FindElements(ByAll.CssSelector("[id*=region]"));
            IList<string> ListaNazwRegionow = new List<string>();

            foreach (var region in ListaRegionow)
            {
                ListaNazwRegionow.Add(region.Text);
            }

            // === przejście przez każdy region
            foreach (var regionNazwa in ListaNazwRegionow)
            {
                IWebElement LinkRegion = Driver.FindElement(By.LinkText(regionNazwa));
                LinkRegion.Click();
                System.Threading.Thread.Sleep(2000);

                //IWebElement AktywnyRegion = Driver.FindElement(By.LinkText(regionNazwa));
                IList<IWebElement> Liga = Driver.FindElements(By.CssSelector("[id*=league]"));

                IList<string> ListaRozgrywekRegionow = new List<string>();
                foreach (var rozgrywka in Liga)
                {
                    ListaRozgrywekRegionow.Add(rozgrywka.Text);
                }

                // przejście przez każdą rozgrywkę
                foreach (var rozgrywkaNazwa in ListaRozgrywekRegionow)
                {
                    IWebElement LinkRozgrywka = Driver.FindElement(By.LinkText(rozgrywkaNazwa));
                    if (rozgrywkaNazwa != "")
                    { 
                        LinkRozgrywka.Click();

                        var wait1 = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                        wait1.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("btn-primary")));
                        System.Threading.Thread.Sleep(2000);

                        IWebElement LinkWszystkie = Driver.FindElement(By.PartialLinkText("Wszystkie ("));
                        LinkWszystkie.Click();

                        var wait2 = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                        wait2.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("btn-primary")));
                        System.Threading.Thread.Sleep(1000);

                        //wyodrebnij tablicę z zakladami
                        IWebElement tablicaZakladow = Driver.FindElement(By.ClassName("bet_tab"));
                        IList<IWebElement> listaBetow = tablicaZakladow.FindElements(By.CssSelector("[data-odds-id]"));
                        Console.WriteLine(listaBetow[2].GetAttribute("data-odds-id"));
                        Console.WriteLine(listaBetow[2].GetAttribute("onclick"));
                        Console.WriteLine(listaBetow[2].GetAttribute(By.XPath("a[onclick *= '12345678901234567890123456789123_12']").ToString()));

                        Console.Write("haha");
                    }
                }

                Console.Write("haha");
            }
            //Console.Write("haha");
        }
    }
}

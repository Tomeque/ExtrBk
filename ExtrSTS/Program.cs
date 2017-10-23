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
                        System.Threading.Thread.Sleep(5000);

                        //wyodrebnij tablicę z zakladami
                        IWebElement tablicaZakladow = Driver.FindElement(By.Id("offerTables"));

                        IList<IWebElement> listaShadowBoxSBO = tablicaZakladow.FindElements(By.XPath("//div[starts-with(@class, 'shadow_box support_bets_offer')]"));
                        foreach (var shadowBox in listaShadowBoxSBO)
                        {
                            IWebElement naglowekZwijany = shadowBox.FindElement(By.TagName("h2"));
                            string h2_header = naglowekZwijany.Text;

                            //IWebElement naglowekOpisowy = shadowBox.FindElement(By.TagName("thead"));
                            //string thead_header = naglowekOpisowy.Text;

                            IList<IWebElement> naglowekPlusRekordy = shadowBox.FindElements(By.XPath("//td[@data-odds-id]|//thead"));

                            foreach (var naglowekLubRekord in naglowekPlusRekordy)
                            {
                                if (naglowekLubRekord.TagName == "thead") {string naglowekOpisowy = naglowekLubRekord.Text;}

                                if (naglowekLubRekord.TagName == "td")
                                {
                                    string data_odds_id = naglowekLubRekord.GetAttribute("data-odds-id"); //id betu
                                    string listaBetProperties = naglowekLubRekord.GetAttribute("onclick");

                                    string oppty_info_number = listaBetProperties.Substring(listaBetProperties.IndexOf("oppty_info_number") + 20, listaBetProperties.IndexOf("id_odds") - listaBetProperties.IndexOf("oppty_info_number") - 23);
                                    string odds_value = listaBetProperties.Substring(listaBetProperties.IndexOf("odds_value") + 13, listaBetProperties.IndexOf("id_opportunity") - listaBetProperties.IndexOf("odds_value") - 16);
                                    string id_opportunity = listaBetProperties.Substring(listaBetProperties.IndexOf("id_opportunity") + 17, listaBetProperties.IndexOf("match_name") - listaBetProperties.IndexOf("id_opportunity") - 20);
                                    string match_name = listaBetProperties.Substring(listaBetProperties.IndexOf("match_name") + 13, listaBetProperties.IndexOf("tip_name_formated") - listaBetProperties.IndexOf("match_name") - 16);
                                    string oppty_type_name = listaBetProperties.Substring(listaBetProperties.IndexOf("oppty_type_name") + 18, listaBetProperties.IndexOf("})") - listaBetProperties.IndexOf("oppty_type_name") - 19);


                                    //SQLiteConnection m_dbConnection;
                                }
                            }

                        }

                        //IList<IWebElement> listaBetow = tablicaZakladow.FindElements(By.CssSelector("[data-odds-id]"));
                        ////Console.Write(listaBetow[5].Text);
                        //foreach (var bet in listaBetow)
                        //{
                        //    //string data_odds_id = bet.GetAttribute("data-odds-id"); //id betu
                        //    //string listaBetProperties = bet.GetAttribute("onclick");
                        //    ////var number = listaBetProperties.IndexOf("oppty_info_number\":\"");
                        //    ////var h1 = listaBetProperties.IndexOf("id_odds");
                        //    ////var h2 = listaBetProperties.IndexOf("oppty_info_number");
                        //    //string oppty_info_number = listaBetProperties.Substring(listaBetProperties.IndexOf("oppty_info_number") + 20, listaBetProperties.IndexOf("id_odds") - listaBetProperties.IndexOf("oppty_info_number") - 23);
                        //    //string odds_value = listaBetProperties.Substring(listaBetProperties.IndexOf("odds_value") + 13, listaBetProperties.IndexOf("id_opportunity") - listaBetProperties.IndexOf("odds_value") - 16);
                        //    //string id_opportunity = listaBetProperties.Substring(listaBetProperties.IndexOf("id_opportunity") + 17, listaBetProperties.IndexOf("match_name") - listaBetProperties.IndexOf("id_opportunity") - 20);
                        //    //string match_name = listaBetProperties.Substring(listaBetProperties.IndexOf("match_name") + 13, listaBetProperties.IndexOf("tip_name_formated") - listaBetProperties.IndexOf("match_name") - 16);
                        //    //string oppty_type_name = listaBetProperties.Substring(listaBetProperties.IndexOf("oppty_type_name") + 18, listaBetProperties.IndexOf("})") - listaBetProperties.IndexOf("oppty_type_name") - 19);

                        //    //IWebElement uprzedniThead = Driver.FindElement(By.XPath("//td[@data-odds-id=" + data_odds_id + "]//.."));
                        //    //IWebElement uprzedniThead = tablicaZakladow.FindElement(By.XPath("//td[@data-odds-id=" + data_odds_id + "]/ancestor"));
                        //    //IList<IWebElement> uprzedniThead = tablicaZakladow.FindElements(By.XPath("ancestor::thead"));
                        //    //IList<IWebElement> uprzedniThead = tablicaZakladow.FindElements(By.XPath("//self"));
                        //    //IWebElement uprzedniThead = tablicaZakladow.FindElement(By.XPath("//thead"));

                        //    //IList<IWebElement> uprzedniThead = tablicaZakladow.FindElements(By.XPath("//..//td[@data-odds-id=" + data_odds_id + "]"));
                        //    //IList<IWebElement> uprzedniThead = tablicaZakladow.FindElements(By.XPath("//parent[.//td[@data-odds-id=" + data_odds_id + "]]"));
                        //    //IList<IWebElement> uprzedniThead = tablicaZakladow.FindElements(By.XPath("/parent[.//td[@data-odds-id=" + data_odds_id + "]]"));
                        //}

                        Console.Write("haha");
                    }
                }

                Console.Write("haha");
            }
            //Console.Write("haha");
        }
    }
}

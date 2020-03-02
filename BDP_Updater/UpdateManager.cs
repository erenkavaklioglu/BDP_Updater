using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace BDP_Updater
{
    public static class UpdateManager
    {
        #region Delegates

        public delegate void NotifyUserDelegate(string title, string text);

        #endregion

        #region Fields

        private static string _URL = @"https://ebeyanname.gib.gov.tr/download.html";

        private static string _downloadAddress = @"https://ebeyanname.gib.gov.tr/ebyn.exe";
        private static string _downloadFileName = @"ebyn";
        private static string _downloadFileExtension = @"exe";
        private static string _downloadFolder = @"C:\ebyn\";

        private static string _exeFileName = @"ebyn.exe";
        private static string _prefixToRemove = @"BDP(Güncellenme Tarihi: ";
        private static string _postfixToRemove = @")";

        private static WebClient _client = null;
        private static string _updatedFilePath;

        private static NotifyUserDelegate NotifyUser;

        #endregion

        #region Properties

        private static WebClient Client
        {
            get
            {
                if (null == _client)
                {
                    _client = new WebClient();
                    _client.DownloadFileCompleted += _client_DownloadFileCompleted;
                }

                return _client;
            }
        }

        #endregion

        #region EventHandlers

        private static void _client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            NotifyUser("Güncelleme işlemi!", "İndirme tamamlandı. Program kuruluyor...");

            ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateCallback));
        }

        #endregion

        #region Methods

        public static void AssignNotifyUserDelegate(NotifyUserDelegate notifyUserMethod)
        {
            NotifyUser = notifyUserMethod;
        }

        public static bool CheckForUpdates(RegistryManager regManager)
        {
            bool result = false;

            string content = Client.DownloadString(_URL);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);

            int day = 0, month = 0, year = 0;

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                if (_exeFileName == link.Attributes[0].Value)
                {
                    string line = link.ParentNode.InnerText;

                    line = line.Replace(_prefixToRemove, "");
                    line = line.Replace(_postfixToRemove, "");

                    string[] dayMonthYear = line.Split('.');

                    day = Convert.ToInt32(dayMonthYear[0]);
                    month = Convert.ToInt32(dayMonthYear[1]);
                    year = Convert.ToInt32(dayMonthYear[2]);

                    result = IsFileUpdatedOnWeb(day, month, year, regManager);
                    break;
                }
            }

            if (result)
            {
                DownloadFile(day, month, year);

                regManager.UpdateDay(day);
                regManager.UpdateMonth(month);
                regManager.UpdateYear(year);
            }

            return result;
        }

        private static bool IsFileUpdatedOnWeb(int webDay, int webMonth, int webYear, RegistryManager regManager)
        {
            bool result = false;

            DateTime webDate = new DateTime(webYear, webMonth, webDay);
            DateTime regDate = new DateTime(regManager.GetYear(), regManager.GetMonth(), regManager.GetDay());

            if (webDate > regDate)
            {
                result = true;
            }

            return result;
        }

        private static void DownloadFile(int day, int month, int year)
        {
            string filePostFix = year.ToString() + month.ToString() + day.ToString();
            _updatedFilePath = _downloadFolder + _downloadFileName + filePostFix + "." + _downloadFileExtension;

            Directory.CreateDirectory(_downloadFolder);

            Client.DownloadFileAsync(new Uri(_downloadAddress), _updatedFilePath);
        }

        private static void UpdateCallback(object state = null)
        {
            Process prc = Process.Start(_updatedFilePath, "/auto");

            while (!prc.HasExited) 
            {
                
            }

            NotifyUser("Güncelleme İşlemi!", "Kurulum tamamlandı.");
        }

        #endregion
    }
}

using Microsoft.Win32;
using System;

namespace BDP_Updater
{
    public class RegistryManager
    {
        #region Fields

        private string _baseKey = @"Software\BDPUpdater";
        private string _yearKey = "Year";
        private string _monthKey = "Month";
        private string _dayKey = "Day";

        private RegistryKey _regKey;

        #endregion

        #region Constructors

        public RegistryManager()
        {
            AddKeyToRegistryIfNotExist();
        }

        #endregion

        #region Methods

        public int GetDay()
        {
            return GetValueAsInteger(_dayKey);
        }

        public int GetMonth()
        {
            return GetValueAsInteger(_monthKey);
        }

        public int GetYear()
        {
            return GetValueAsInteger(_yearKey);
        }

        
        public void UpdateDay(int day)
        {
            AddValueToRegistry(_regKey, _dayKey, day.ToString());
        }

        public void UpdateMonth(int month)
        {
            AddValueToRegistry(_regKey, _monthKey, month.ToString());
        }

        public void UpdateYear(int year)
        {
            AddValueToRegistry(_regKey, _yearKey, year.ToString());
        }


        private int GetValueAsInteger(string key)
        {
            int result = 0;
            int day;

            if (Int32.TryParse(GetValue(key), out day))
            {
                result = day;
            }

            return result;
        }

        /// <summary>
        /// Registry içerisine kayıt ekler
        /// </summary>
        /// <param name="key">Kayıt eklenecek registry</param>
        /// <param name="value">Kayıt değeri</param>
        private void AddValueToRegistry(RegistryKey regKey, string keyValue, string value, bool checkForNullControl = false)
        {
            if (null != regKey)
            {
                if (checkForNullControl)
                {
                    if (null == regKey.GetValue(keyValue))
                    {
                        regKey.SetValue(keyValue, value);
                    }
                }
                else
                {
                    regKey.SetValue(keyValue, value);
                }
            }
        }

        /// <summary>
        /// Key değeri yok ise registry içerisine kaydını ekler
        /// </summary>
        private void AddKeyToRegistryIfNotExist()
        {
            RegistryKey localMachine;

            if (Environment.Is64BitOperatingSystem)
            {
                localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
            }

            _regKey = localMachine.OpenSubKey(_baseKey, true);

            if (_regKey == null)
            {
                _regKey = localMachine.CreateSubKey(_baseKey);
            }

            AddValueToRegistry(_regKey, _yearKey, "2000", true);
            AddValueToRegistry(_regKey, _monthKey, "01", true);
            AddValueToRegistry(_regKey, _dayKey, "01", true);
        }

        /// <summary>
        /// Registry'den değeri alır
        /// </summary>
        /// <param name="key">Key değeri</param>
        /// <returns>Değer var ise değeri, yoksa boş string döndürür</returns>
        private string GetValue(string key)
        {
            string result = string.Empty;

            if (null != _regKey)
            {
                object value = _regKey.GetValue(key);

                if (null != value)
                {
                    result = value.ToString();
                }
            }

            return result;
        }

        #endregion
    }
}

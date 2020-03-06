using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SysTimer = System.Timers.Timer;

namespace BDP_Updater
{
    public partial class FormMain : Form
    {
        #region Fields

        private RegistryManager _regManager;

        private SysTimer _updateCheckTimer;

        private bool _updateActive;

        private bool _fileIsUpToDate;

        #endregion

        #region Constructors

        public FormMain()
        {
            InitializeComponent();

            _updateActive = false;
            _fileIsUpToDate = false;
            _regManager = new RegistryManager();
            UpdateManager.AssignNotifyUserDelegate(NotifyUser);

            _updateCheckTimer = new SysTimer();
            _updateCheckTimer.Interval = 20000;//600000; //10 dakika
            _updateCheckTimer.Elapsed += _updateCheckTimer_Elapsed;
            _updateCheckTimer.Start();
        }

        #endregion

        #region EventHandlers

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void _updateCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_updateActive)
            {
                _updateActive = true;

                if (UpdateManager.CheckForUpdates(_regManager))
                {
                    NotifyUser("Dosya Güncelleme!", "Yeni bir güncelleme bulundu.");
                    _fileIsUpToDate = false;
                }
                else if (!_fileIsUpToDate)
                {
                    NotifyUser("BDP Durumu", "Beyanname Düzenleme Programı güncel!");
                    _fileIsUpToDate = true;
                }

                _updateActive = false;
            }
        }

        #endregion

        #region Methods

        private void NotifyUser(string title, string text)
        {
            NotifyIcon downloadNotification = new NotifyIcon();

            downloadNotification.Visible = true;
            downloadNotification.Icon = SystemIcons.Warning;
            downloadNotification.Text = text;
            downloadNotification.BalloonTipTitle = title;
            downloadNotification.BalloonTipText = text;
            downloadNotification.BalloonTipIcon = ToolTipIcon.Warning;
            downloadNotification.ShowBalloonTip(3000);
            
            //downloadNotification.Visible = false;
        }

        #endregion
    }
}

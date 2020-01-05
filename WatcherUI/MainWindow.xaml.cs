using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using WatcherUI.Utils;

namespace WatcherUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private WinProcessWatcher esoProcess = new WinProcessWatcher("eso64");
        private WinProcessWatcher ahkProcess = new WinProcessWatcher("esoAHK", GeneralSettings.Default.exeLocation);

        public MainWindow()
        {
            InitializeComponent();
            _ahkLocationTextBox.Text = GeneralSettings.Default.ahkLocation;
            _exeLocationTextBox.Text = GeneralSettings.Default.exeLocation;

            esoProcess.OnEveryTick += EsoProcess_OnEveryTick;
            esoProcess.OnProcessAlive += EsoProcess_OnProcessAlive;
            esoProcess.OnProcessDead += EsoProcess_OnProcessDead;

            ahkProcess.OnEveryTick += AhkProcess_OnEveryTick;
        }

        private void _ahkLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettings.Default.ahkLocation = Utils.FileDialog.GetFilePath("Autohotkey Scripts (*.ahk)|*.ahk", GeneralSettings.Default.ahkLocation);
            GeneralSettings.Default.Save();
        }

        private void _exeLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            GeneralSettings.Default.exeLocation = Utils.FileDialog.GetFilePath("Executable Files (*.exe)|*.exe", GeneralSettings.Default.exeLocation);
            GeneralSettings.Default.Save();
        }

        private void _ahkCompilerLocationBrowseButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GeneralSettings.Default.ahkCompilerLocation = Utils.FileDialog.GetFilePath("Executable Files (*.exe)|*.exe", GeneralSettings.Default.ahkCompilerLocation);
            GeneralSettings.Default.Save();
        }

        private void _exeCompileButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                StartLoading();
                Process.Start(GeneralSettings.Default.ahkCompilerLocation, $"/in {GeneralSettings.Default.ahkLocation} /out {GeneralSettings.Default.exeLocation}");
                EndLoading();
            });
        }

        private void PackIconBoxIcons_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", GeneralSettings.Default.ahkLocation);
        }

        private bool isWatcherActive = false;
        private void _watcherButton_Click(object sender, RoutedEventArgs e)
        {
            if(!isWatcherActive)
            {
                isWatcherActive = true;
                _watcherButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.BlurOn;
                _watcherButtonText.Text = "Watcher ON";
                ahkProcess.StartWatching();
                esoProcess.StartWatching();
            }
            else
            {
                isWatcherActive = false;
                _watcherButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialDesignKind.BlurOff;
                _watcherButtonText.Text = "Watcher OFF";
                ahkProcess.StopWatching();
                esoProcess.StopWatching();
                _ahkProcessStatusIcon.Kind = PackIconEvaIconsKind.QuestionMarkCircleOutline;
                _esoProcessStatusIcon.Kind = PackIconEvaIconsKind.QuestionMarkCircleOutline;
            }
        }

        private void AhkProcess_OnEveryTick()
        {
            SetStatus(_ahkProcessStatusIcon, ahkProcess.IsActive);
        }

        private void EsoProcess_OnProcessDead()
        {
            if (ahkProcess.IsActive)
                ahkProcess.Kill();
        }

        private void EsoProcess_OnProcessAlive()
        {
            if (!ahkProcess.IsActive)
                ahkProcess.Start();
        }

        private void EsoProcess_OnEveryTick()
        {
            SetStatus(_esoProcessStatusIcon, esoProcess.IsActive);
        }

        private void StartLoading(int sleepTimeMS = 500)
        {
            _loading.Invoke(() => _loading.Visibility = Visibility.Visible);
            _loadingShield.Invoke(() => _loadingShield.Spin = true);
            Thread.Sleep(sleepTimeMS);
        }

        private void EndLoading()
        {
            _loading.Invoke(() => _loading.Visibility = Visibility.Hidden);
        }

        private void SetStatus(PackIconEvaIcons statusIcon, bool status)
        {
            statusIcon.Invoke(() => statusIcon.Kind = status ? PackIconEvaIconsKind.CheckmarkCircle2Outline : PackIconEvaIconsKind.CloseCircleOutline);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Toolbar().Show();
        }
    }
}

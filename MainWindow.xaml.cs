﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Switch_Trados_Studios_Environment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OverwriteStudioEnvironment overwriteStudioEnvironment = new OverwriteStudioEnvironment();
        private readonly InstalledBuilds installedBuilds = new InstalledBuilds();
        private readonly StudioProcess studioProcess = new StudioProcess();
        private readonly LanguageCloudConfig languageCloudConfig = new LanguageCloudConfig();

        public MainWindow()
        {
            InitializeComponent();
            
            foreach (var build in installedBuilds.listOfInstalledBuilds)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Background = Brushes.White;
                comboBoxItem.Content = $"{build.DisplayName} {build.DisplayVersion}";
                StudioBuildType.Items.Add(comboBoxItem);
            }
            if (installedBuilds.listOfInstalledBuilds.Count == 1)
            {
                StudioBuildType.SelectedIndex = 0;
            }
            else if (installedBuilds.listOfInstalledBuilds.Count == 0)
            {
                const string message = "There isn't any Trados Studio Build installed\n\nPlease Install one and restart the tool or add a custom path";
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.DarkRed;
                SwitchEnvironmentButton.IsEnabled = false;
            }

            var environmentConfigFiles = EnvironmentFiles.GetEnvironmentFilesDictionaries();

            foreach (string environmentFile in environmentConfigFiles)
            {
                string fileName = new FileInfo(environmentFile).Name;
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Background = Brushes.White;
                comboBoxItem.Content = System.IO.Path.GetFileNameWithoutExtension(fileName);
                EnvironmentType.Items.Add(comboBoxItem);
            }
        }

        private void ChangeEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var studioIndex = StudioBuildType.SelectedIndex;
            var environmentIndex = EnvironmentType.SelectedIndex;
            var studioProcesses = studioProcess.GetStudioProcesses();

            if (studioIndex == -1 || environmentIndex == -1)
            {
                const string message = "Select which Trados Studio build you want to change \n\nAnd what environment type you want to change to";
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.Black;
                return;
            }

            if (studioProcesses.Length != 0)
            {
                const string message = "Trados Studio needs to be closed before changing the environment. \n\nDo you want to close it?";
                const string caption = "Close Studio Confirmation";
                var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                studioProcess.CloseStudio(studioProcesses);
            }

            languageCloudConfig.DeleteLanguageCloudConfig(studioIndex);

            SwitchEnvironmentButton.IsEnabled = false;
            try
            {
                string message = $"Success:\n\n{StudioBuildType.Text} has been changed to the {EnvironmentType.Text}";
                overwriteStudioEnvironment.SwitchStudiosLcEnvironment(studioIndex, environmentIndex);
                Status.Visibility = Visibility.Visible;
                Status.Text = message;
                Status.Foreground = Brushes.Green;
            }
            catch(Exception exception)
            {
                Status.Visibility = Visibility.Visible;
                Status.Text = $"Failed:\n\n{exception.Message}";
                Status.Foreground = Brushes.OrangeRed;
            }
            SwitchEnvironmentButton.IsEnabled = true;
        }

        private void CustomLocation_Click(object sender, RoutedEventArgs e)
        {
            string customLocation = EnvironmentFiles.AddCustomLocation();
            if (customLocation != string.Empty)
            {
                bool customPathAdded = AddCustomBuildPathToDictionary(customLocation);
                if (customPathAdded)
                {
                    StudioBuildType.Items.Add(customLocation);
                }
                StudioBuildType.SelectedItem = customLocation;
                SwitchEnvironmentButton.IsEnabled = true;
            }
        }

        private bool AddCustomBuildPathToDictionary(string customPath)
        {
            bool valueExists = installedBuilds.studioBuildTypeDictionary.Values.Contains<string>(customPath);
            if (!valueExists)
            {
                int Key = installedBuilds.studioBuildTypeDictionary.Count;
                overwriteStudioEnvironment.installedBuilds.studioBuildTypeDictionary.Add(Key, customPath);
                return true;
            }
            return false;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://confluence.sdl.com/pages/viewpage.action?spaceKey=LTSTUDIO&title=Switch+Trados+Studio%27s+Environment",
                UseShellExecute = true
            });
        }
    }
}

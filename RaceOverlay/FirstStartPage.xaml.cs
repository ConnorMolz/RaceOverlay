using System.IO;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace RaceOverlay;

public partial class FirstStartPage : Window
{
    public FirstStartPage()
    {
        InitializeComponent();
        LoadLicense();
    }
    
    /// <summary>
    /// Loads the LICENSE and Manual resources from the assembly and displays them in the respective text blocks.
    /// </summary>
    private void LoadLicense()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            string resourceNameLicense = "RaceOverlay.Resources.LICENSE";
                
            using (Stream stream = assembly.GetManifestResourceStream(resourceNameLicense))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        LicenseText.Text = reader.ReadToEnd();
                    }
                }
                else
                {
                    LicenseText.Text = "LICENSE resource not found in the application.";
                }
            }
            string resourceNameManual = "RaceOverlay.Resources.Manual";
                
            using (Stream stream = assembly.GetManifestResourceStream(resourceNameManual))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        ManualText.Text = reader.ReadToEnd();
                    }
                }
                else
                {
                    LicenseText.Text = "LICENSE resource not found in the application.";
                }
            }
        }
        catch (Exception ex)
        {
            LicenseText.Text = $"Error loading LICENSE resource: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Handle the click event of the Accept button.
    /// Safes the settings to indicate that the first run has been completed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void On_Accept_Button(object sender, RoutedEventArgs e)
    {
        string settingsFilePath = Path.Combine(App.AppDataPath, "settings.json");
        string jsonContent = File.ReadAllText(settingsFilePath);
        JObject settingsObject = JObject.Parse(jsonContent);
        settingsObject["FirstRun"] = false;
        File.WriteAllText(settingsFilePath, settingsObject.ToString());
        
        Close();
    }
}

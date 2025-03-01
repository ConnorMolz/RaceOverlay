using System.IO;
using System.Reflection;
using System.Windows;

namespace RaceOverlay;

public partial class FirstStartPage : Window
{
    public FirstStartPage()
    {
        InitializeComponent();
        LoadLicense();
    }

    private void LoadLicense()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            string resourceName = "RaceOverlay.Resources.LICENSE";
                
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
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
        }
        catch (Exception ex)
        {
            LicenseText.Text = $"Error loading LICENSE resource: {ex.Message}";
        }
    }
}

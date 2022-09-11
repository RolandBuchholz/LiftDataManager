using System.Xml;

namespace LiftDataManager.ViewModels;

public partial class QuickLinksViewModel : DataViewModelBase, INavigationAware
{
    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenSpeziPdfCommand))]
    private bool canOpenSpeziPdf;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenVaultCommand))]
    private bool canOpenVault;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenCFPCommand))]
    private bool canOpenCFP;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OpenLiloCommand))]
    private bool canOpenLilo;

    private void CheckCanOpenFiles()
    {

        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml"))
        {
            CanOpenSpeziPdf = File.Exists(FullPathXml.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf"));
        }

        CanOpenVault = !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml");

        var bausatztyp = "";

        if (ParamterDictionary!["var_Bausatz"].Value is not null)
        {
            bausatztyp = ParamterDictionary["var_Bausatz"].Value;
        }

        CanOpenCFP = bausatztyp is "BRR-15 MK2" or "BRR-25 MK2" or "ZZE-S1600";
        CanOpenLilo = bausatztyp.StartsWith("BR1") || bausatztyp.StartsWith("BR2") || bausatztyp.StartsWith("BT") || bausatztyp.StartsWith("TG");
    }

    [RelayCommand]
    private void OpenSpezi()
    {
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var filename = @"C:\Work\Administration\Spezifikation\Spezifikation.xlsm";
        var startargs = "";

        if (!string.IsNullOrWhiteSpace(auftragsnummer))
        {
            filename = @"C:\Program Files (x86)\Microsoft Office\Office16\EXCEL.EXE";
            startargs = @"/e/" + auftragsnummer + @" C:\Work\Administration\Spezifikation\Spezifikation.xlsm";
        }

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenSpeziPdf))]
    private void OpenSpeziPdf()
    {
        var filename = FullPathXml?.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf");
        var startargs = "";

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    [RelayCommand]
    private void OpenBauer()
    {
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var filename = @"C:\Work\Administration\Tools\Explorer Start.exe";

        if (File.Exists(FullPathXml))
        {
            if (auftragsnummer != null)
            {
                StartProgram(filename, auftragsnummer);
            }
        }
    }

    [RelayCommand]
    private void OpenWorkspace()
    {
        if (!string.IsNullOrWhiteSpace(FullPathXml))
        {
            var pathXml = Path.GetDirectoryName(FullPathXml);
            var filename = "explorer.exe";

            if (Directory.Exists(pathXml))
            {
                StartProgram(filename, pathXml);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenVault))]
    private void OpenVault()
    {
        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml"))
        {
            MakeVaultLink(FullPathXml);
            var vaultLink = @"C:\Temp\VaultLink.acr";
            var startargs = "";
            if (File.Exists(FullPathXml))
            {
                StartProgram(vaultLink, startargs);
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenCFP))]
    private void OpenCFP()
    {
        var auftragsnummer = ParamterDictionary?["var_AuftragsNummer"].Value;
        var bausatztyp = ParamterDictionary?["var_Bausatz"].Value;
        var user = Environment.GetEnvironmentVariable("userprofile");
        var cfpPath = user + @"\AppData\Local\Bausatzauslegung\CFP\UpdateCFP.exe";
        var startargs = auftragsnummer + " " + bausatztyp;
        using Process p = new();

        StartProgram(cfpPath, startargs);
    }

    [RelayCommand]
    private void OpenZiehlAbegg()
    {
        var filename = @"C:\Program Files (x86)\zetalift\Lift.exe";
        var startargs = "";

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    [RelayCommand(CanExecute = nameof(CanOpenLilo))]
    private void OpenLilo()
    {
        var filename = @"C:\Program Files (x86)\BucherHydraulics\LILO\PRG\LILO.EXE";
        var startargs = "";

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }

        var user = Environment.GetEnvironmentVariable("userprofile");
        var cfpPath = user + @"\AppData\Local\Bausatzauslegung\CFP\UpdateCFP.exe";
        using Process p = new();

        if (File.Exists(filename))
        {
            StartProgram(cfpPath, startargs);
        }
    }

    private static void StartProgram(string filename, string startargs)
    {
        using Process p = new();
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.FileName = filename;
        p.StartInfo.Arguments = startargs;
        p.Start();
    }

    private static void MakeVaultLink(string path)
    {
        var vaultPath = path.Replace(@"C:\Work", "$").Replace(@"\", "/");

        XmlWriter? oXmlWriter = null;
        var oXmlWriterSettings = new XmlWriterSettings();

        try
        {
            // Eigenschaften / Einstellungen festlegen
            oXmlWriterSettings.Indent = true;
            oXmlWriterSettings.IndentChars = "  ";
            oXmlWriterSettings.NewLineChars = "\r\n";

            oXmlWriter = XmlWriter.Create(@"C:\Temp\VaultLink.acr", oXmlWriterSettings);

            oXmlWriter.WriteStartElement("ADM", "http://schemas.autodesk.com/msd/plm/ExplorerAutomation/2004-11-01");

            oXmlWriter.WriteStartElement("Server");
            oXmlWriter.WriteString("192.168.0.1");
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteStartElement("Vault");
            oXmlWriter.WriteString("Vault");
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteStartElement("Operations");
            oXmlWriter.WriteStartElement("Operation");
            oXmlWriter.WriteAttributeString("ObjectType", "File");
            oXmlWriter.WriteStartElement("ObjectID");
            oXmlWriter.WriteString(vaultPath);
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteStartElement("Command");
            oXmlWriter.WriteString("Select");
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteEndElement();
            oXmlWriter.WriteEndElement();

            oXmlWriter.WriteEndElement();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
        finally
        {
            oXmlWriter?.Close();
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        SynchronizeViewModelParameter();
        CheckCanOpenFiles();
    }

    public void OnNavigatedFrom()
    {
    }
}
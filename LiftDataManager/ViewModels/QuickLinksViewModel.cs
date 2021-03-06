using System.Xml;

namespace LiftDataManager.ViewModels;

public class QuickLinksViewModel : DataViewModelBase, INavigationAware
{
    public QuickLinksViewModel(IParameterDataService parameterDataService, IDialogService dialogService, INavigationService navigationService) :
         base(parameterDataService, dialogService, navigationService)
    {
        OpenSpeziCommand = new RelayCommand(OpenSpezi);
        OpenSpeziPdfCommand = new RelayCommand(OpenSpeziPdf, () => CanOpenSpeziPdf);
        OpenBauerCommand = new RelayCommand(OpenBauer);
        OpenWorkspaceCommand = new RelayCommand(OpenWorkspace);
        OpenVaultCommand = new RelayCommand(OpenVault, () => CanOpenVault);
        OpenCFPCommand = new RelayCommand(OpenCFP, () => CanOpenCFP);
        OpenZiehlAbeggCommand = new RelayCommand(OpenZiehlAbegg);
        OpenLiloCommand = new RelayCommand(OpenLilo, () => CanOpenLilo);
    }

    public IRelayCommand OpenSpeziCommand
    {
        get;
    }
    public IRelayCommand OpenSpeziPdfCommand
    {
        get;
    }
    public IRelayCommand OpenBauerCommand
    {
        get;
    }
    public IRelayCommand OpenWorkspaceCommand
    {
        get;
    }
    public IRelayCommand OpenVaultCommand
    {
        get;
    }
    public IRelayCommand OpenCFPCommand
    {
        get;
    }
    public IRelayCommand OpenZiehlAbeggCommand
    {
        get;
    }
    public IRelayCommand OpenLiloCommand
    {
        get;
    }

    private bool _CanOpenSpeziPdf;
    public bool CanOpenSpeziPdf
    {
        get => _CanOpenSpeziPdf;
        set
        {
            SetProperty(ref _CanOpenSpeziPdf, value);
            OpenSpeziPdfCommand.NotifyCanExecuteChanged();
        }
    }

    private bool _CanOpenVault;
    public bool CanOpenVault
    {
        get => _CanOpenVault;
        set
        {
            SetProperty(ref _CanOpenVault, value);
            OpenVaultCommand.NotifyCanExecuteChanged();
        }
    }
    private bool _CanOpenCFP;
    public bool CanOpenCFP
    {
        get => _CanOpenCFP;
        set
        {
            SetProperty(ref _CanOpenCFP, value);
            OpenCFPCommand.NotifyCanExecuteChanged();
        }
    }

    private bool _CanOpenLilo;
    public bool CanOpenLilo
    {
        get => _CanOpenLilo;
        set
        {
            SetProperty(ref _CanOpenLilo, value);
            OpenLiloCommand.NotifyCanExecuteChanged();
        }
    }

    private void CheckCanOpenFiles()
    {

        if (!string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml"))
        {
            CanOpenSpeziPdf = File.Exists(FullPathXml.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf"));
        }

        CanOpenVault = !string.IsNullOrWhiteSpace(FullPathXml) && (FullPathXml != @"C:\Work\Administration\Spezifikation\AutoDeskTransfer.xml");

        var bausatztyp = "";

        if (ParamterDictionary["var_Bausatz"].Value is not null)
        {
            bausatztyp = ParamterDictionary["var_Bausatz"].Value;
        }

        CanOpenCFP = bausatztyp is "BRR-15 MK2" or "BRR-25 MK2" or "ZZE-S1600";
        CanOpenLilo = bausatztyp.StartsWith("BR1") || bausatztyp.StartsWith("BR2") || bausatztyp.StartsWith("BT") || bausatztyp.StartsWith("TG");
    }

    private void OpenSpezi()
    {
        var auftragsnummer = ParamterDictionary["var_AuftragsNummer"].Value;
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

    private void OpenSpeziPdf()
    {
        var filename = FullPathXml.Replace("-AutoDeskTransfer.xml", "-Spezifikation.pdf");
        var startargs = "";

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

    private void OpenBauer()
    {
        var auftragsnummer = ParamterDictionary["var_AuftragsNummer"].Value;
        var filename = @"C:\Work\Administration\Tools\Explorer Start.exe";

        if (File.Exists(FullPathXml))
        {
            StartProgram(filename, auftragsnummer);
        }
    }

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

    private void OpenCFP()
    {
        var auftragsnummer = ParamterDictionary["var_AuftragsNummer"].Value;
        var bausatztyp = ParamterDictionary["var_Bausatz"].Value;
        var user = Environment.GetEnvironmentVariable("userprofile");
        var cfpPath = user + @"\AppData\Local\Bausatzauslegung\CFP\UpdateCFP.exe";
        var startargs = auftragsnummer + " " + bausatztyp;
        using Process p = new();

        StartProgram(cfpPath, startargs);
    }

    private void OpenZiehlAbegg()
    {
        var filename = @"C:\Program Files (x86)\zetalift\Lift.exe";
        var startargs = "";

        if (File.Exists(filename))
        {
            StartProgram(filename, startargs);
        }
    }

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

        XmlWriter oXmlWriter = null;
        XmlWriterSettings oXmlWriterSettings = new XmlWriterSettings();

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
            oXmlWriter.Close();
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

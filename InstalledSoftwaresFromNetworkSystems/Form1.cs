using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace InstalledSoftwaresFromNetworkSystems
{
    /// <summary>
    /// This is tested in windows 7.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtSystemName.Text))
                {
                    MessageBox.Show("Please enter system name");
                    return;
                }

                CoumputerInfo computerInfo = new CoumputerInfo();
                computerInfo.ComputerName = txtSystemName.Text;
                computerInfo.UserName = txtUserName.Text;
                computerInfo.Password = txtPassword.Text;
                List<Software> programs = this.ReadRemoteRegistryusingWMI(computerInfo);
                this.GenerateFile(programs, computerInfo);
                this.ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ClearFields()
        {
            txtSystemName.Text = string.Empty;
            txtUserName.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private List<Software> ReadRemoteRegistryusingWMI(CoumputerInfo computerInfo)
        {
            List<Software> programs = new List<Software>();

            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Username = computerInfo.UserName;
            connectionOptions.Password = computerInfo.Password;
            //connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope scope = new ManagementScope("\\\\" + computerInfo.ComputerName + "\\root\\CIMV2", connectionOptions);
            scope.Connect();

            string softwareRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

            ManagementClass registry = new ManagementClass(scope, new ManagementPath("StdRegProv"), null);
            ManagementBaseObject inParams = registry.GetMethodParameters("EnumKey");
            inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
            inParams["sSubKeyName"] = softwareRegLoc;

            // Read Registry Key Names 
            ManagementBaseObject outParams = registry.InvokeMethod("EnumKey", inParams, null);
            string[] programGuids = outParams["sNames"] as string[];

            foreach (string subKeyName in programGuids)
            {
                inParams = registry.GetMethodParameters("GetStringValue");
                inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
                inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;
                inParams["sValueName"] = "DisplayName";
                // Read Registry Value 
                outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                Software softWare = new Software();
                if (outParams.Properties["sValue"].Value != null)
                {
                    string softwareName = outParams.Properties["sValue"].Value.ToString();
                    if (!softwareName.ToLower().Contains("security update for") &&
                        !softwareName.ToLower().Contains("update for windows") &&
                        !softwareName.ToLower().Contains("hotfix for"))
                    {

                        softWare.Name = softwareName;
                        inParams = registry.GetMethodParameters("GetStringValue");
                        inParams["hDefKey"] = 0x80000002;//HKEY_LOCAL_MACHINE
                        inParams["sSubKeyName"] = softwareRegLoc + @"\" + subKeyName;
                        inParams["sValueName"] = "Publisher";
                        // Read Registry Value 
                        outParams = registry.InvokeMethod("GetStringValue", inParams, null);
                        if (outParams.Properties["sValue"].Value != null)
                        {
                            softWare.Publisher = outParams.Properties["sValue"].Value.ToString();
                        }
                        else
                        {
                            softWare.Publisher = "Un-known";
                        }

                        programs.Add(softWare);
                    }
                }
            }

            return programs;
        }

        private bool GenerateFile(List<Software> softwareList, CoumputerInfo computerInfo)
        {
            using (StreamWriter sw = new StreamWriter("installedSoftwares_" + computerInfo.ComputerName + ".csv", false, Encoding.Unicode))
            {
                int i = 1;
                sw.Write("Sl.No");
                sw.Write('\t');
                sw.Write("Software Installed");
                sw.Write('\t');
                sw.Write("Publisher");
                sw.Write('\t');
                sw.Write("Machine name");
                sw.Write('\t');
                sw.Write("Is required for work");
                sw.Write('\t');
                sw.Write(sw.NewLine);
                foreach (var software in softwareList)
                {
                    sw.Write(i);
                    sw.Write('\t');
                    sw.Write(software.Name);
                    sw.Write('\t');
                    sw.Write(software.Publisher);
                    sw.Write('\t');
                    sw.Write(computerInfo.ComputerName);
                    sw.Write('\t');
                    sw.Write(@"Yes\No");
                    sw.Write(sw.NewLine);
                    i++;
                }

                sw.Close();
            }

            MessageBox.Show("Software list .csv file is created");
        }
    }
}

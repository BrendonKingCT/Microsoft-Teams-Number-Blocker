using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace NumberBlocker
{
    public partial class Form1 : Form
    {
        private Runspace runspace;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            outBox.AppendText("Connecting..." + "\n");
            btnConnect.Enabled = false;

            string tenantId = txtTenantId.Text;

            // Run the connection to Microsoft Teams asynchronously
            await Task.Run(() => ConnectToTeams(tenantId));

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = true;
        }

        private void ConnectToTeams(string tenantId)
        {
            try
            {
                // Create PowerShell Runspace
                runspace = RunspaceFactory.CreateRunspace();
                runspace.Open();

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.Runspace = runspace;

                    // Set the execution policy for the current process
                    ps.AddScript("Set-ExecutionPolicy RemoteSigned -Scope Process -Force");
                    ps.Invoke();

                    // Check for errors during setting the execution policy
                    if (ps.HadErrors)
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nFailed to set execution policy: " + string.Join(", ", ps.Streams.Error.Select(e => e.ToString())) + "\n");
                        }));
                        return;
                    }

                    // Import the Microsoft Teams module
                    ps.Commands.Clear();
                    ps.AddScript("Import-Module MicrosoftTeams");
                    ps.Invoke();

                    // Check for errors during module import
                    if (ps.HadErrors)
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nModule Import Failed: " + string.Join(", ", ps.Streams.Error.Select(e => e.ToString()))+"\n");
                        }));
                        return;
                    }

                    // Use device authentication for logging in via the browser
                    ps.Commands.Clear();
                    ps.AddCommand("Connect-MicrosoftTeams")
                      .AddParameter("TenantId", tenantId);
                      //.AddParameter("UseDeviceAuthentication");

                    // Execute the PowerShell command to connect to Teams
                    var results = ps.Invoke();

                    // Check for errors
                    if (ps.HadErrors)
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nLogin Failed: " + string.Join(", ", ps.Streams.Error.Select(e => e.ToString())) + "\n");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nLogin Successful!\n");
                            groupBox2.Enabled = true;
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    outBox.AppendText("Error: " + ex.Message + "\n");
                }));
            }
        }

        private void DisconnectFromTeams()
        {
            try
            {
                // Create PowerShell Runspace if not already created
                if (runspace == null)
                {
                    runspace = RunspaceFactory.CreateRunspace();
                    runspace.Open();
                }

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.Runspace = runspace;

                    // Command to disconnect from Microsoft Teams
                    ps.AddCommand("Disconnect-MicrosoftTeams");

                    // Execute the PowerShell command to disconnect
                    ps.Invoke();

                    // Check for errors
                    if (ps.HadErrors)
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nDisconnect Failed: " + string.Join(", ", ps.Streams.Error.Select(e => e.ToString())) + "\n");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nDisconnected Successfully!\n");
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    outBox.AppendText("\nError: " + ex.Message + "\n");
                }));
            }
            finally
            {
                // Close the runspace if it's open
                if (runspace != null)
                {
                    runspace.Close();
                    runspace = null; // Reset runspace for the next connection
                }
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectFromTeams();
        }

        private void CreateBlockedNumberPattern()
        {
            try
            {
                // Ensure that the runspace is created and opened
                if (runspace == null)
                {
                    runspace = RunspaceFactory.CreateRunspace();
                    runspace.Open();
                }

                using (PowerShell ps = PowerShell.Create())
                {
                    ps.Runspace = runspace;

                    // Get values from the form
                    string pattern = txtPattern.Text.Trim();
                    string identity = "Number Block: " + pattern;
                    string description = txtDescription.Text.Trim();
                    bool isEnabled = radioEnabled.Checked;

                    // Validate input
                    if (string.IsNullOrEmpty(pattern))
                    {
                        MessageBox.Show("Please enter a valid number pattern.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Prepare the command to create a blocked number pattern
                    ps.AddCommand("New-CsInboundBlockedNumberPattern")
                      .AddParameter("Identity", identity)
                      .AddParameter("Pattern", pattern)
                      .AddParameter("Description", description)
                      .AddParameter("Enabled", isEnabled);

                    // Execute the PowerShell command
                    var results = ps.Invoke();

                    // Check for errors
                    if (ps.HadErrors)
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nFailed to create blocked number pattern: " + string.Join(", ", ps.Streams.Error.Select(e => e.ToString())) + "\n");
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            outBox.AppendText("\nBlocked number pattern created successfully!\n");
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    outBox.AppendText("Error: " + ex.Message);
                }));
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            CreateBlockedNumberPattern();
        }
    }
}

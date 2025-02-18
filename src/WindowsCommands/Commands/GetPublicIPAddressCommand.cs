using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsCommands.Commands
{
    [OutputType(typeof(IPAddress))]
    [OutputType(typeof(string))]
    [Cmdlet(VerbsCommon.Get, PublicIPAddressName)]
    public class GetPublicIPAddressCommand : PSCmdlet
    {
        private const string PublicIPAddressName = CmdletStrings.PublicIPAddressName;

        [Parameter]
        public SwitchParameter AsString { get; set; }

        [Parameter]
        public SwitchParameter Clip { get; set;  }

        protected override void ProcessRecord()
        {
            string externalIpString = new WebClient()
                .DownloadString("http://icanhazip.com")
                .Replace("\\r\\n", "")
                .Replace("\\n", "")
                .Trim();
            var externalIp = IPAddress.Parse(externalIpString);

            if (externalIp is not null)
            {

                if (!AsString)
                {
                    WriteObject(externalIp);
                } else
                {
                    WriteObject(externalIp.ToString());
                }

                if (Clip)
                {
                    Clipboard.SetText(externalIp.ToString());
                }

            }
            else
            {
                WriteWarning("Could not get public IP Address. Check your connection and try again.");
            }               
        }
    }
}

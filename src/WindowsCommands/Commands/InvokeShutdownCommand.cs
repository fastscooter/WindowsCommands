using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management;

namespace WindowsCommands.Commands
{
    [Cmdlet(VerbsLifecycle.Invoke, ShutdownName, SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class InvokeShutdownCommand : PSCmdlet
    {
        private const string ShutdownName = CmdletStrings.ShutdownName;
        private bool _shouldProcess = true;

        [Parameter(Mandatory = true)]
        public ShutdownReason Reason { get; set; }

        private readonly Dictionary<string, object> PSBoundParameters;

        public InvokeShutdownCommand()
        {
            PSBoundParameters = MyInvocation.BoundParameters;
        }


        protected override void BeginProcessing()
        {
            bool keyValue = false;
            if(PSBoundParameters.ContainsKey("Confirm"))
            {
                keyValue = (bool)MyInvocation.BoundParameters["Confirm"];
            }
            if (!keyValue)
            {
                _shouldProcess = ShouldProcess(System.Net.Dns.GetHostName());
            }
        }

        protected override void ProcessRecord()
        {
            if (_shouldProcess)
            {
                var managementClass = GetManagementClass();

                var parameters = UpdateParameters(managementClass);

                var instance = managementClass.CreateInstance();

                instance.InvokeMethod("Win32Shutdown", parameters, null);
            }
        }

        private ManagementBaseObject UpdateParameters(ManagementClass managementClass)
        {
            var parameters = managementClass.GetMethodParameters("Win32Shutdown");
            parameters["Flags"] = $"{(int)Reason}";
            parameters["Reserved"] = "0";
            return parameters;
        }

        private static ManagementClass GetManagementClass()
        {
            var managementClass = new ManagementClass("Win32_OperatingSystem");
            managementClass.Get();
            managementClass.Scope.Options.EnablePrivileges = true;
            return managementClass;
        }

        public override string ToString() => CmdletStrings.InvokeShutdownCmdlet;
    }

    public enum ShutdownReason
    {
        Reboot = 0x00000002,
        PowerOff = 0x00000008,
        Logoff = 0x00000000
    }
}

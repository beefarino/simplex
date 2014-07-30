using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using CodeOwls.ScriptProvider.Persistence;
using CodeOwls.PowerShell.Paths;
using CodeOwls.PowerShell.Paths.Processors;

namespace CodeOwls.ScriptProvider.Provider
{
    [CmdletProvider( "Simplex", ProviderCapabilities.ShouldProcess )]
    public class ScriptProviderProvider : CodeOwls.PowerShell.Provider.Provider
    {
        public ScriptProviderDrive Drive
        {
            get { return this.PSDriveInfo as ScriptProviderDrive ?? this.ProviderInfo.Drives.FirstOrDefault() as ScriptProviderDrive; }
        }

        protected override IPathResolver PathResolver
        {
            get { return new ScriptProviderPathResolver( Drive ); }
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            return new ScriptProviderDrive(
                drive,
                new ScriptPersister(drive.Root, this)
                );
        }
        
    }
}

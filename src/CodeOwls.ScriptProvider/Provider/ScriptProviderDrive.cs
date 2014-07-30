using System.Management.Automation;
using CodeOwls.PowerShell.Provider;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Nodes;
using CodeOwls.ScriptProvider.Persistence;

namespace CodeOwls.ScriptProvider.Provider
{
    public class ScriptProviderDrive : Drive
    {
        private readonly IPersistScriptProviderNode _persister;

        public ScriptProviderDrive(PSDriveInfo driveInfo, IPersistScriptProviderNode persister) : base(driveInfo)
        {
            _persister = persister;
        }

        public IFolder RootFolder
        {
            get {return Persister.Load("") as IFolder;}
        }

        public IPersistScriptProviderNode Persister { get { return _persister; } }
    }
}
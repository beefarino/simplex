using System.Collections.Generic;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IScript : IFolder
    {
        IEnumerable<PSObject> Invoke( IProviderContext context );
        string IdField { get;  }
        IEnumerable<IItem> Inputs { get; }
        ScriptBlock Script { get; }        
    }
}
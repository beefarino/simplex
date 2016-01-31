using System.Collections.Generic;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;

namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IScript : IFolder
    {
        IEnumerable<PSObject> Invoke( IProviderContext context );
        PSObject InvokeAdd(IProviderContext context, string itemName, string type, object value);
        void InvokeRemove(IProviderContext context, string itemName);
        string IdField { get;  }
        IEnumerable<IItem> Inputs { get; }
        ScriptBlock Script { get; }
        ScriptBlock AddScript { get; }
        ScriptBlock RemoveScript { get; }
    }
}
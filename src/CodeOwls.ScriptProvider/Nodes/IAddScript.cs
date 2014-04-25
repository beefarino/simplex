using System.Collections.Generic;
using System.Management.Automation;

namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IAddScript
    {
        IFolder AddScript(string name, ScriptBlock scriptBlock, string idField );
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Nodes;

namespace CodeOwls.ScriptProvider.Persistence
{
    public interface IPersistScriptProviderNode
    {
        bool Remove(IItem item);
        bool Save(IItem item);
        IItem Load(string moniker);
    }
}

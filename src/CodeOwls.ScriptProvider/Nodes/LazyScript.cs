using System.Collections.Generic;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Persistence;

namespace CodeOwls.ScriptProvider.Nodes
{
    public class LazyScript : IScript
    {
        private readonly string _path;
        private readonly IPersistScriptProviderNode _persister;
        private IScript _script;
        private IScript RealScript
        {
            get
            {
                if( null == _script)
                {
                    _script = _persister.Load(_path) as IScript;
                }
                return _script;
            }
        }

        public LazyScript( string path, IPersistScriptProviderNode persister )
        {
            _path = path;
            _persister = persister;
        }

        public NodeType NodeType
        {
            get { return RealScript.NodeType; }
        }

        public string Name
        {
            get { return RealScript.Name; }
        }

        public IFolder ParentFolder
        {
            get { return RealScript.ParentFolder; }
        }

        public PSObject Value
        {
            get { return RealScript.Value; }
        }

        public IEnumerable<IItem> Children
        {
            get { return RealScript.Children; }
        }

        public IEnumerable<PSObject> Invoke(IProviderContext context)
        {
            return RealScript.Invoke(context);
        }

        public string IdField
        {
            get { return RealScript.IdField; }
        }

        public IEnumerable<IItem> Inputs { get { return RealScript.Inputs;  } }

        public ScriptBlock Script
        {
            get { return RealScript.Script; }
        }

        /*public IDictionary<string, ScriptBlock> MenuItems 
        {
            get { return RealScript.MenuItems; }
        }*/
    }
}
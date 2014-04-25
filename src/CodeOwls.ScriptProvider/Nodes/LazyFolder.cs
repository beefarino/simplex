using System.Collections.Generic;
using System.Management.Automation;
using CodeOwls.ScriptProvider.Persistence;

namespace CodeOwls.ScriptProvider.Nodes
{
    public class LazyFolder : IFolder
    {
        private readonly string _path;
        private readonly IPersistScriptProviderNode _persister;
        private IFolder _folder;

        public LazyFolder( string path, IPersistScriptProviderNode persister )
        {
            _path = path;
            _persister = persister;
        }

        IFolder Folder
        {
            get
            {
                if( null == _folder )
                {
                    _folder = _persister.Load(_path) as IFolder;
                }
                return _folder;
            }
        }
        
        public NodeType NodeType { get { return Folder.NodeType; } }
        public string Name { get { return Folder.Name; } }
        public IFolder ParentFolder { get { return Folder.ParentFolder; } }
        public PSObject Value { get { return Folder.Value; } }
        public IEnumerable<IItem> Children { get { return Folder.Children; } }
    }
}
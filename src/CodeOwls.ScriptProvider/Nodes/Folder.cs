using System.Collections.Generic;
using System.Management.Automation;

namespace CodeOwls.ScriptProvider.Nodes
{
    public class Folder : IFolder, IAddFolder, IAddScript, IRemoveFolderItem
    {
        protected List<IItem> _children;

        public Folder( string name )
        {
            Name = name;

            Value = this.AsPSObject();
            _children = new List<IItem>();
            NodeType = NodeType.Folder;
        }

        public NodeType NodeType { get; protected set; }
        public IFolder ParentFolder { get; private set; }
        public PSObject Value { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<IItem> Children { get { return _children; } }
        
        public void AddChildItem( IItem child )
        {
            _children.Add( child );
        }

        public IFolder AddFolder(string name)
        {
            var folder = new Folder(name);
            _children.Add( folder );
            return folder;
        }

        public IFolder AddScript( string name, string script )
        {
            var scriptFolder = new ScriptFolder(name, script, null);
            _children.Add( scriptFolder  );
            return scriptFolder;
        }

        public IFolder AddScript(string name, ScriptBlock scriptBlock, string idField, ScriptBlock addScriptBlock, ScriptBlock removeScriptBlock )
        {
            var scriptFolder = new ScriptFolder(name, scriptBlock, idField, addScriptBlock, removeScriptBlock );
            _children.Add( scriptFolder);
            return scriptFolder;
        }

        public void Remove(IItem item)
        {
            _children.Remove(item);
        }
    }
}
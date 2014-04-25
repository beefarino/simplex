using System;
using CodeOwls.ScriptProvider.Nodes;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class ItemPathNode : PathNodeBase
    {
        private readonly IItem _folder;

        public ItemPathNode(IItem folder)
        {
            _folder = folder;
        }

        public override IPathValue GetNodeValue()
        {
            return new LeafPathValue( _folder, Name );
        }

        public override string Name
        {
            get { return _folder.Name; }
        }
        
        public static IPathNode Create(ScriptProviderDrive drive, IItem item)
        {
            switch (item.NodeType)
            {
                case (NodeType.Folder):
                    {
                        return new FolderPathNode(drive, item as IFolder);
                    }
                case(NodeType.Script):
                    {
                        return new ScriptPathNode(drive, item as IScript);
                    }
                case( NodeType.Root):
                    {
                        return new RootPathNode( drive );
                    }
                default:
                    {
                        throw new NotSupportedException( "the specified node type is not yet supported");
                    }

            }
        }
    }
}
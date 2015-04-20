using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CodeOwls.ScriptProvider.Nodes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class FolderPathNode: PathNodeBase, INewItem, IRemoveItem
    {
        private readonly ScriptProviderDrive _drive;
        private readonly IFolder _folder;

        public FolderPathNode(ScriptProviderDrive drive, IFolder folder)
        {
            _drive = drive;
            _folder = folder;
        }

        public override IPathValue GetNodeValue()
        {
            return new ContainerPathValue(_folder, Name);
        }

        public override string Name
        {
            get { return _folder.Name; }
        }

        public override IEnumerable<IPathNode> GetNodeChildren(PowerShell.Provider.PathNodeProcessors.IProviderContext context)
        {
            return _folder.Children.ToList().ConvertAll(a=>ItemPathNode.Create(_drive,a));
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return new[]{"script", "folder"}; }
        }
        public object NewItemParameters { get { return null; } }
        public IPathValue NewItem(IProviderContext context, string path, string itemTypeName, object newItemValue)
        {
            if (null == path)
            {
                var err = new ErrorRecord(
                    new ArgumentException("The new item must have a valid item name.  Specify the new item name as part of the -path parameter, or specify the -name parameter.", "Path"),
                    "ScriptProvider.NewItem.MustHaveName",
                    ErrorCategory.InvalidArgument,
                    path);
                context.WriteError( err );
                return null;
            }

            IPathValue node = null;
            if (null == itemTypeName)
            {
                if (newItemValue is ScriptBlock)
                {
                    itemTypeName = "script";
                }
            }

            switch( itemTypeName.ToLowerInvariant())
            {
                case("script"):
                    {
                        if (null == newItemValue || String.IsNullOrWhiteSpace(newItemValue.ToString()))
                        {
                            var err = new ErrorRecord(
                                new ArgumentException("Script items must have a value.  Specify the script item value as a string or scriptblock in the -value parameter, or specify the -name parameter.", "Value"),
                                "ScriptProvider.NewItem.ScriptsMustHaveValue",
                                ErrorCategory.InvalidArgument,
                                path
                            );
                            context.WriteError(err);
                            return null;
                        }
                        node = NewScript(path, newItemValue);
                        break;
                    }
                case("folder"):
                default:
                    {
                        node = NewFolder(path);
                        break;
                    }
            }
            return node;
        }

        protected ScriptProviderDrive Drive
        {
            get { return _drive; }
        }

        private IPathValue NewFolder(string path)
        {
            var folder = _folder as IAddFolder;
            if( null == folder )
            {
                throw new NotSupportedException();
            }
            
            var newfolder = folder.AddFolder(path);
            Persist(newfolder);
            return new ContainerPathValue( newfolder, newfolder.Name );
        }

        private void Persist(IFolder newfolder)
        {
            Drive.Persister.Save(newfolder);
        }
        private void Unpersist(IFolder newfolder)
        {
            Drive.Persister.Remove(newfolder);
        }
        private IPathValue NewScript(string name, object newItemValue)
        {
            if( null == newItemValue )
            {
                throw new ArgumentNullException( "newItemValue");
            }
            var folder = _folder as IAddScript;

            if (null == folder)
            {
                throw new NotSupportedException();
            }

            var scriptBlock = newItemValue as ScriptBlock;
            var script = ScriptBlock.Create( newItemValue.ToString() );
            //todo: fix
            var newScript = folder.AddScript(name, scriptBlock ?? script, null);
            Persist(newScript);
            return new ContainerPathValue( newScript, name );
        }

        public object RemoveItemParameters { get; private set; }
        public void RemoveItem(IProviderContext context, string path, bool recurse)
        {
            var re = new Regex( @"[\/\\]+[^\/\\]+$");
            var parentPath = re.Replace(context.Path, String.Empty);
            var parent = context.ResolvePath(parentPath);
            if (null == parent)
            {
                return;
            }

            var folder = parent.GetNodeValue().Item as IRemoveFolderItem;//_folder.ParentFolder as IRemoveFolderItem;
            if (null == folder)
            {
                return;
            }
            folder.Remove(_folder);
            Unpersist(_folder);
        }
    }
}
using CodeOwls.ScriptProvider.Nodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class RootPathNode : FolderPathNode
    {
        public RootPathNode(ScriptProviderDrive drive) : base(drive, drive.RootFolder as IFolder)
        {
        }
    }
}
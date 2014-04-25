using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class PathResolver : PathResolverBase
    {
        private readonly ScriptProviderDrive _drive;

        public PathResolver(ScriptProviderDrive drive)
        {
            _drive = drive;
        }

        protected override IPathNode Root
        {
            get { return new RootPathNode( _drive ); }
        }
    }
}
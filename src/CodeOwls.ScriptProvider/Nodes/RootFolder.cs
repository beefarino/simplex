namespace CodeOwls.ScriptProvider.Nodes
{
    public class RootFolder : Folder
    {
        public RootFolder(string name) : base( name )
        {
            NodeType = NodeType.Root;
        }
    }
}
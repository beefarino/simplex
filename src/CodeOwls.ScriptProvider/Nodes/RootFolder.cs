namespace CodeOwls.ScriptProvider.Nodes
{
    public class RootFolder : Folder
    {
        public RootFolder(string name) : base( name, null )
        {
            NodeType = NodeType.Root;
        }
    }
}
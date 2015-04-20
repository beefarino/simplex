using System.Management.Automation;

namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IItem
    {
        NodeType NodeType { get; }
        string Name { get; }
        //IFolder ParentFolder { get; }
        PSObject Value { get; }
    }
}
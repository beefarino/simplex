using System.Collections.Generic;

namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IFolder : IItem
    {
        IEnumerable<IItem> Children { get; }        
    }
}
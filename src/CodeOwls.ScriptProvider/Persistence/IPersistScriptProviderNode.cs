using CodeOwls.ScriptProvider.Nodes;

namespace CodeOwls.ScriptProvider.Persistence
{
    public interface IPersistScriptProviderNode
    {
        bool Remove(IItem item);
        bool Save(IItem item);
        IItem Load(string moniker);
    }
}

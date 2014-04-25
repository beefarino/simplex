namespace CodeOwls.ScriptProvider.Nodes
{
    public interface IAddFolder
    {
        IFolder AddFolder(string name);
    }

    public interface IRemoveFolderItem
    {
        void Remove(IItem item);
    }
}
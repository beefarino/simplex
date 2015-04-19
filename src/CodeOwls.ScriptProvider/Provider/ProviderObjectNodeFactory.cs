using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class ProviderObjectNodeFactory : PathNodeBase, IInvokeItem, IClearItem, IRemoveItem, IRenameItem, INewItem, ISetItem
    {
        private readonly PSObject _pso;

        public ProviderObjectNodeFactory( PSObject pso )
        {
            _pso = pso;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            if (!IsOriginalObjectAContainer)
            {
                return null;
            }

            var pspath = OriginalObjectPSPath;
            if (String.IsNullOrWhiteSpace(pspath))
            {
                return null;
            }

            var objects = providerContext.InvokeProvider.ChildItem.Get( new [] {pspath}, providerContext.Recurse, providerContext.Force, false);
            objects.UpdateProperties( providerContext );

            var factories = objects.ToList().ConvertAll(o => new ProviderObjectNodeFactory(o));
            return factories;
        }

        public override IPathValue GetNodeValue()
        {            
            if (IsOriginalObjectAContainer)
            {
                return new ContainerPathValue( _pso, Name );
            }
            return new LeafPathValue( _pso, Name);
        }
        
        public override string Name
        {
            get { return _pso.SafeGetPropertyValue<object>(ScriptProviderPropertyNames.ChildName, ()=>_pso.ToString()).ToString(); }
        }


        string OriginalObjectPSPath
        {
            get
            {
                return _pso.SafeGetPropertyValue<string>(ScriptProviderPropertyNames.Path, (string)null ); 
            }
        }

        bool IsOriginalObjectAContainer
        {
            get { return _pso.SafeGetPropertyValue<bool>(ScriptProviderPropertyNames.IsContainer, false ); }
        }

        public IEnumerable<object> InvokeItem(IProviderContext providerContext, string path)
        {
            providerContext.InvokeProvider.Item.Invoke( OriginalObjectPSPath );
            return null;
        }

        public object InvokeItemParameters { get; private set; }
        
        public void ClearItem(IProviderContext providerContext, string path)
        {
            providerContext.InvokeProvider.Item.Clear(OriginalObjectPSPath);
        }

        public object ClearItemDynamicParamters { get; private set; }
        
        public void RemoveItem(IProviderContext providerContext, string path, bool recurse)
        {
            providerContext.InvokeProvider.Item.Remove(new[] { OriginalObjectPSPath }, providerContext.Recurse, providerContext.Force, false);
        }

        public object RemoveItemParameters { get; private set; }

        public void RenameItem(IProviderContext providerContext, string path, string newName)
        {
            providerContext.InvokeProvider.Item.Rename(OriginalObjectPSPath, newName, providerContext.Force);
        }

        public object RenameItemParameters { get; private set; }
        
        public IPathValue NewItem(IProviderContext providerContext, string path, string itemTypeName, object newItemValue)
        {
            var newItem = providerContext.InvokeProvider.Item.New( new[]{OriginalObjectPSPath}, path, itemTypeName, newItemValue,
                providerContext.Force);

            return this.Resolve( providerContext, path ).First().GetNodeValue();
        }

        public IEnumerable<string> NewItemTypeNames { get; private set; }
        
        public object NewItemParameters { get; private set; }

        public IPathValue SetItem(IProviderContext providerContext, string path, object value)
        {
            providerContext.InvokeProvider.Item.Set(new[]{OriginalObjectPSPath}, value, providerContext.Recurse, false);
            return this.Resolve(providerContext, path).First().GetNodeValue();
        }

        public object SetItemParameters { get; private set; }
    }
}
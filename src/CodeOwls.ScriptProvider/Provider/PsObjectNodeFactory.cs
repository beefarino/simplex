using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class PsObjectNodeFactory : PathNodeBase
    {
        private readonly PSObject _pso;

        public PsObjectNodeFactory( PSObject pso )
        {
            _pso = pso;
        }

        public override IPathValue GetNodeValue()
        {
            return new LeafPathValue( _pso, Name);
        }
        
        public override string Name
        {
            get { return _pso.SafeGetPropertyValue<object>("ScriptProviderChildName", ()=>_pso.ToString()).ToString(); }
        }
    }
}
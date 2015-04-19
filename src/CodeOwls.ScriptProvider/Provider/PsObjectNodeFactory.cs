using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    internal class PsObjectNodeFactory : PathNodeBase
    {
        private readonly PSObject _pso;

        public PsObjectNodeFactory(PSObject pso)
        {
            _pso = pso;
        }

        public override IPathValue GetNodeValue()
        {
            return new LeafPathValue(_pso, Name);
        }

        public override string Name
        {
            get
            {
                return
                    _pso.SafeGetPropertyValue<object>(ScriptProviderPropertyNames.ChildName, () => _pso.ToString())
                        .ToString();
            }
        }
    }
}
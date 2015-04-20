using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CodeOwls.ScriptProvider.Nodes;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class ScriptPathNode : FolderPathNode
    {
        private readonly IScript _script;

        public ScriptPathNode( ScriptProviderDrive drive, IScript script ) : base(drive, script )
        {
            _script = script;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(PowerShell.Provider.PathNodeProcessors.IProviderContext context)
        {
            var list = new List<IPathNode>(base.GetNodeChildren(context));

            var results = _script.Invoke(context).ToList();
            UpdateProperties(context, results);
            
            var psoResults = results.ConvertAll(CreateNode);

            list.AddRange(psoResults);
            
            return list;
        }

        private IPathNode CreateNode(PSObject input)
        {
            
            if (input.BaseObject is IScript)
            {
                return new ScriptPathNode( Drive, input.BaseObject as IScript);
            }
            if (input.BaseObject is IFolder)
            {
                return new FolderPathNode(Drive, input.BaseObject as IFolder);
            }

            if (input.Properties.Match("PSPath").Any())
            {
                return new ProviderObjectNodeFactory(input);
            }

            return new PsObjectNodeFactory(input);
        }

        public override string Name
        {
            get { return _script.Name; }
        }

        private void UpdateProperties(IProviderContext context, List<PSObject> results)
        {
            context.WriteDebug( "updating item properties" );
            results.ForEach(r => r.Properties.ToList().ForEach(p => context.WriteDebug(p.Name)));

            results.ForEach(r => r.Properties.Where(a => a.Name.StartsWith("PS")).ToList().ForEach(
                p =>
                    {
                        context.WriteDebug(p.Name);
                        var i = new PSNoteProperty(p.Name.ToScriptProviderPropertyName(), p.Value);
                        r.Properties.Add(i);

                    }
                  )
                );
        }

    }
}
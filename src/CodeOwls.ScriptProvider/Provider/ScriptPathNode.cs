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
            
            var psoResults = results.ConvertAll(i => new PsObjectNodeFactory(i)).Cast<IPathNode>();

            list.AddRange(psoResults);
            
            return list;
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
                        var i = new PSNoteProperty("ScriptProviderItem" + p.Name, p.Value);
                        r.Properties.Add(i);

                    }
                  )
                );


            /*results.ForEach(r => r.Properties.Where(a => a.Name == "MenuItems" ).ToList().ForEach(
                p =>
                {
                    context.WriteDebug(p.Name);
                    var i = new PSNoteProperty("ScriptProviderMenuItems" + p.Name, p.Value);
                    r.Properties.Add(i);

                }
                                     )
                );*/
        }

    }
}
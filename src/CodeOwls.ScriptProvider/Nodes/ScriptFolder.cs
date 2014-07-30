using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;


namespace CodeOwls.ScriptProvider.Nodes
{
    public class ScriptFolder : IScript
    {
        private readonly string _name;
        private readonly ScriptBlock _script;
        private readonly string _idField;

        private static readonly ScriptBlock DefaultConvertToFile = ScriptBlock.Create("$input | convertto-xml -as string");
        public ScriptFolder(string name, string script, string idField, IFolder parentFolder)
            : this( name, ScriptBlock.Create(script), idField,parentFolder)
        {
        }

        public ScriptFolder(string name, ScriptBlock script, string idField, IFolder parentFolder)
        {
            ParentFolder = parentFolder;
            _name = name;
            _script = script;
            _idField = idField;
            Inputs = new List<IItem>();
            NodeType = NodeType.Script;
            ConvertToFile = DefaultConvertToFile;
        }

        public string IdField { get { return _idField; } }
        public ScriptBlock ConvertToFile { get; set; }

        public IEnumerable<PSObject> Invoke( IProviderContext context )
        {
            var r = context.SessionState.InvokeCommand.InvokeScript(false, _script, null).ToList();
                r.ForEach(q =>
                    {
                        if (!String.IsNullOrWhiteSpace(_idField))
                        {
                            if (!q.Properties.Any(p => p.Name.ToLowerInvariant() == "ScriptProviderchildname"))
                            {
                                var value = q.SafeGetPropertyValue<object>(_idField,
                                                                           () =>
                                                                           q.SafeGetPropertyValue<object>(
                                                                               "PSChildName", () => null));
                                if (null != value)
                                {
                                    var prop = new PSNoteProperty("ScriptProviderChildName", value);
                                    q.Properties.Add(prop);
                                }
                            }
                        }
                    });

                context.WriteDebug( String.Format("Invocation of [{0}] complete; [{1}] results returned", _script, r.Count) );
                return r;
            
        }

        private static void SetPSObjectProperty(string pname, string pvalue, PSObject q)
        {
            if (! q.Properties.Any(p => p.Name.ToLowerInvariant() == pname.ToLowerInvariant()))
            {
                var prop = new PSNoteProperty(pname, pvalue);
                q.Properties.Add(prop);
            }
        }

        public NodeType NodeType { get; private set; }

        public string Name
        {
            get { return _name; }
        }

        public IEnumerable<IItem> Inputs { get; private set; } 

        public IEnumerable<IItem> Children
        {
            get
            {
                var list = new List<IItem>();
                list.AddRange( Inputs );

                return list;
            }
        }

        public ScriptBlock Script
        {
            get { return _script; }
        }

        public PSObject Value
        {
            get { return _script.AsPSObject(); }
        }

        public IFolder ParentFolder
        {
            get; private set;
        }
    }
}

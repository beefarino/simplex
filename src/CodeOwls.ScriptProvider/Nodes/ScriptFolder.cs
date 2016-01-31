using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Provider;


namespace CodeOwls.ScriptProvider.Nodes
{
    public class ScriptFolder : IScript
    {
        private readonly string _name;
        private readonly ScriptBlock _getScript;
        private readonly ScriptBlock _addScript;
        private readonly ScriptBlock _removeScript;
        private readonly string _idField;

        private static readonly ScriptBlock DefaultConvertToFile = ScriptBlock.Create("$input | convertto-xml -as string");

        public ScriptFolder(string name, string script, string idField)
            : this( name, ScriptBlock.Create(script), idField, null, null)
        {
        }


        public ScriptFolder(string name, string script, string idField, string addScript, string removeScript)
            : this(name, ScriptBlock.Create(script), idField, ScriptBlock.Create(addScript), ScriptBlock.Create(removeScript))
        {
        }

        public ScriptFolder(string name, ScriptBlock getScript, string idField, ScriptBlock addScript, ScriptBlock removeScript)
        {
            _name = name;
            _getScript = getScript;
            _addScript = addScript;
            _removeScript = removeScript;
            _idField = idField;
            Inputs = new List<IItem>();
            NodeType = NodeType.Script;
            ConvertToFile = DefaultConvertToFile;
        }

        public string IdField { get { return _idField; } }
        public ScriptBlock ConvertToFile { get; set; }

        public IEnumerable<PSObject> Invoke( IProviderContext context )
        {
            var r = context.SessionState.InvokeCommand.InvokeScript(false, _getScript, null).ToList();
                r.ForEach( UpdateIdProperty );

                context.WriteDebug( String.Format("Invocation of [{0}] complete; [{1}] results returned", _getScript, r.Count) );
                return r;
            
        }

        public PSObject InvokeAdd(IProviderContext context, string itemName, string type, object value)
        {
            var r = context.SessionState.InvokeCommand.InvokeScript(false, _addScript, null, new []{value}).ToList();
            r.ForEach(UpdateIdProperty);

            context.WriteDebug(String.Format("Invocation of [{0}] complete; [{1}] results returned", _getScript, r.Count));
            return r.FirstOrDefault();
        }


        public void InvokeRemove(IProviderContext context, string itemName)
        {
            context.SessionState.InvokeCommand.InvokeScript(false, _removeScript, null, new[] { itemName }).ToList();
            
        }
        
        private void UpdateIdProperty(PSObject q)
        {
            if (!String.IsNullOrWhiteSpace(_idField))
            {
                var matches = q.Properties.Match(ScriptProviderPropertyNames.ChildName);
                if (matches.IsNullOrEmpty())
                {
                    var pvalue = q.SafeGetPropertyValue<object>(_idField, () => q.SafeGetPropertyValue<object>("PSChildName", () => null));
                    if (null != pvalue)
                    {
                        var prop = new PSNoteProperty(ScriptProviderPropertyNames.ChildName, pvalue);
                        q.Properties.Add(prop);
                    }
                }
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
            get { return _getScript; }
        }

        public ScriptBlock AddScript
        {
            get { return _addScript; }
        }

        public ScriptBlock RemoveScript
        {
            get { return _removeScript; }
        }

        public PSObject Value
        {
            get { return _getScript.AsPSObject(); }
        }

    }
}

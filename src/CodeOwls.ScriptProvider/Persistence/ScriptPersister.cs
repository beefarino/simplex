using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Nodes;

namespace CodeOwls.ScriptProvider.Persistence
{
    public class ScriptPersister : IPersistScriptProviderNode
    {
        private readonly string _scriptPath;
        private readonly PSObject _root;
        
        public ScriptPersister( string scriptPath, CodeOwls.PowerShell.Provider.Provider provider )
        {
            _scriptPath = scriptPath;
            _root = provider.SessionState.InvokeCommand.InvokeScript(scriptPath).FirstOrDefault();
        }

        public bool Remove(IItem item)
        {
            return SaveScript();
        }

        public bool Save(IItem item)
        {
            return SaveScript();
        }

        bool SaveScript()
        {
            var builder = new StringBuilder();
            MakeScript( _root.BaseObject as IItem, builder );
            var script = builder.ToString();
            //Clipboard.SetText( script );

            File.WriteAllText( _scriptPath, script);

            return true;
        }

        private int _indent = 0;

        private void MakeScript(IItem item, StringBuilder builder)
        {
            const string rootOpen = @"root {";
            const string rootClose = @"} # end of root";
            const string folderOpenFormat = @"{1}folder '{0}' {{";
            const string folderCloseFormat = @"{0}}} # end of folder";
            const string scriptFormat = @"{1}script '{0}' ";
            
            if (item.NodeType == NodeType.Root)
            {
                _indent = 0;
                builder.AppendLine(rootOpen);
                var folder = item as IFolder;
                ++_indent;
                folder.Children.ToList().ForEach(c=>MakeScript( c, builder ));
                --_indent;
                builder.AppendLine(rootClose);

            }
            else if (item is IScript)
            {
                var s = item as IScript;
                builder.AppendFormat(scriptFormat, s.Name, GetIndent( _indent));
                AppendIfValid(s.IdField, "-idField '{0}' ", builder);
                builder.AppendLine("{");
                builder.AppendFormat("{0}", Indent(s.Script.ToString(), ++_indent) );
                builder.AppendFormat("{0}}}", GetIndent(--_indent));
                builder.AppendLine();
            }
            else if (item is IFolder)
            {
                var f = item as IFolder;
                builder.AppendFormat(folderOpenFormat, f.Name, GetIndent(_indent));
                builder.AppendLine();
                ++_indent;
                f.Children.ToList().ForEach(c=>MakeScript(c, builder));
                --_indent;
                builder.AppendFormat(folderCloseFormat, GetIndent( _indent ));
                builder.AppendLine();
            }
        }

        string Indent(string value, int level)
        {
            var indent = GetIndent(level);
            return indent +
                value.Trim().Replace(Environment.NewLine, Environment.NewLine + indent)
                + Environment.NewLine;
        }
        string GetIndent(int level)
        {
            return new string( ' ' , level );
        }
        void AppendIfValid(string value, string format, StringBuilder builder)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return;
            }

            builder.AppendFormat(format, value);
        }

        private void AddItemToParent(IItem item)
        {
            var parentMoniker = GetMoniker(item.ParentFolder);
            var parentItem = GetDataItem( parentMoniker.Split('/','\\') );
            var folder = parentItem as IFolder;
            if (null == folder)
            {
                throw new InvalidOperationException( "The specified item parent is not a folder");
            }
            if (item.NodeType == NodeType.Folder)
            {
                var addFolder = folder as IAddFolder;
                if (null == addFolder)
                {
                    throw new InvalidOperationException( "The specified parent folder does not support adding child folders");
                }
                addFolder.AddFolder(item.Name);
            }
            else if (item.NodeType == NodeType.Script)
            {
                var addScript = folder as IAddScript;
                if (null == addScript)
                {
                    throw new InvalidOperationException( "The specified parent folder does not support adding child scripts");
                }
                var script = item as IScript;
                addScript.AddScript(
                    script.Name, script.Script, 
                    script.IdField
                );                
            }
        }

        private string GetMoniker(IItem item)
        {
            var ra = new List<string>();
            while (null != item)
            {
                ra.Add( item.Name );
                item = item.ParentFolder;
            }
            ra.Reverse();
            return String.Join("/", ra.Where(v=>!String.IsNullOrWhiteSpace(v)).ToArray());

        }

        public IItem Load(string moniker)
        {            
            if (null == _root)
            {
                return null;
            }

            IFolder parentFolder = null;
            var parentMoniker = Regex.Replace(moniker, @"(^|[\\/])[^\\/]+$", String.Empty);
            if( !String.IsNullOrEmpty(parentMoniker))
            {
                parentFolder = new LazyFolder( parentMoniker, this );
            }

            var parts = moniker.Split('/', '\\');
            var dataItem = GetDataItem(parts);
            if (null == dataItem)
            {
                return null;
            }

            var scriptItem = dataItem as IScript;
            var name = parts.Last();
            if (null == scriptItem)
            {
                var folder = dataItem as IFolder;
                
                return folder;
            }

            return scriptItem;
        }

        private string BuildPath(string moniker, IItem child)
        {
            return Path.Combine(moniker, child.Name);
        }

        bool IsScriptNode(PSObject item, string name)
        {
            return null != item.SafeGetPropertyValue<ScriptBlock>("script", () => null);
        }

        private bool IsScriptNode(PSObject dataItem, PSPropertyInfo child)
        {
            if (null == child)
            {
                return false;
            }
            return IsScriptNode(dataItem, child.Name);
        }

        private IItem  GetDataItem(string[] parts)
        {
            var item = _root.BaseObject as IItem;
            foreach (var part in from p in parts where !String.IsNullOrWhiteSpace(p) select p )
            {
                string childName = part;
                var childFolder = item as IFolder;
                if (null == childFolder)
                {
                    return null;
                }
                var info = from child in childFolder.Children
                           where StringComparer.InvariantCultureIgnoreCase.Equals( child.Name, childName )
                           select child;
                if (! info.Any() )
                {
                    return null;
                }

                var value = info.First();
                if (null == value)
                {
                    return null;
                }
                item = value;
            }
            return item;
        }
    }
}

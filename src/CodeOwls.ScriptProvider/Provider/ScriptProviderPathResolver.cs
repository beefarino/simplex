﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.ScriptProvider.Provider
{
    class ScriptProviderPathResolver : IPathResolver
    {
        private readonly ScriptProviderDrive _drive;

        public ScriptProviderPathResolver(ScriptProviderDrive drive)
        {
            _drive = drive;
        }

        public IEnumerable<IPathNode> ResolvePath(IProviderContext context, string path)
        {
            context.WriteDebug(String.Format("Resolving path [{0}] drive [{1}]", path, context.Drive));
            path = Regex.Replace(path, @"^[^::]+::", String.Empty);
            if (null != context.Drive && !String.IsNullOrEmpty(context.Drive.Root))
            {
                path = Regex.Replace(path, Regex.Escape(context.Drive.Root), String.Empty);
            }

            var item = _drive.Persister.Load(path);
            if( null == item )
            {
                var parts = Regex.Split(path, @"[\\\/]+").ToList();
                var childName = parts.Last();
                parts.RemoveAt( parts.Count - 1 );
                var parentPath = String.Join("\\", parts.ToArray());
                item = _drive.Persister.Load(parentPath);
                if (null == item)
                {
                    return null;
                }

                var factory = ItemPathNode.Create(_drive, item);
                return factory.Resolve(context, childName);
            }

            return new[]{ItemPathNode.Create(_drive,item)};
        }
    }
}
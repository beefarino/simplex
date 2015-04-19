using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.ScriptProvider.Provider;

namespace CodeOwls.ScriptProvider
{
    public static class Extensions
    {
        public static PSObject AsPSObject( this object o )
        {
            return PSObject.AsPSObject(o);
        }

        public static bool HasProperty( this PSObject pso, string name )
        {
            if( null == pso )
            {
                return false;
            }

            return pso.Properties.Match(name).Count > 0;
        }

        public static bool IsNullOrEmpty<T>( this IEnumerable<T> items )
        {
            if (null == items)
            {
                return true;
            }

            return !items.Any();
        }


        public static T Pop<T>(this IList<T> items)
        {
            if (null == items)
            {
                return default (T);
            }

            T item = items.Last();
            items.RemoveAt( items.Count - 1);
            return item;
        }

        public static T SafeGetPropertyValue<T>(this PSObject pso, string name, T defaultValue)
        {
            return pso.SafeGetPropertyValue(name, () => defaultValue);
        }

        public static T SafeGetPropertyValue<T>(this PSObject pso, string name, Func<T> defaultValue)
        {
            T t = default(T);
            if( null == pso )
            {
                return defaultValue();
            }
            try
            {
                var m = pso.Properties.Match(name);
                if (0 == m.Count)
                {
                    return defaultValue();
                }

                var value = m[0].Value;

                try
                {
                    t = (T) value;
                }
                catch (InvalidCastException)
                {
                    t = defaultValue();
                }

                return t;
            }
            catch (Exception e)
            {
                return defaultValue();
            }
        }

        public static T SafeGetPropertyValue<T>(this PSObject pso, string name)
        {
            return pso.SafeGetPropertyValue<T>(name, default(T));
        }

        public static void UpdateProperties(this IEnumerable<PSObject> results, IProviderContext context)
        {
            context.WriteDebug("updating item properties");
            results.ToList().ForEach(result =>
            {
                result.Properties.ToList().ForEach(p => context.WriteDebug(p.Name));

                result.Properties.Where(a => a.Name.StartsWith("PS")).ToList().ForEach(
                    p =>
                    {
                        context.WriteDebug(p.Name);
                        var i = new PSNoteProperty(p.Name.ToScriptProviderPropertyName(), p.Value);
                        result.Properties.Add(i);

                    });
            });
        }
    }
}

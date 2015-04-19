namespace CodeOwls.ScriptProvider.Provider
{
    public static class ScriptProviderPropertyNames
    {
        public static readonly string IsContainer = "PSIsContainer".ToScriptProviderPropertyName();
        public static readonly string Path = "PSPath".ToScriptProviderPropertyName();
        public static readonly string ParentPath = "PSParentPath".ToScriptProviderPropertyName();
        public static readonly string ChildName = "PSChildName".ToScriptProviderPropertyName();
        public static readonly string Drive = "PSDrive".ToScriptProviderPropertyName();
        public static readonly string Provider = "PSProvider".ToScriptProviderPropertyName();

        public static string ToScriptProviderPropertyName(this string originalPropertyName)
        {
            return "ScriptProviderItem" + originalPropertyName;
        }
    }
}
namespace CodeOwls.ScriptProvider
{
    static class LogManager
    {
        static public CodeOwls.PowerShell.Provider.Provider CurrentProvider { get; set; }
        static public void WriteDebug(string msg)
        {
            var p = CurrentProvider;
            if (null != p)
            {
                p.WriteDebug( msg );
            }
        }
    }
}
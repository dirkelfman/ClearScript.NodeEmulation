namespace ClearScript.NodeEmulation
{
    public interface INodeConsole
    {
        void Assert(params object[] stuff);
        void Error(params object[] stuff);
        void Info(params object[] stuff);
        void Log(params object[] stuff);
        void Time(params object[] stuff);
        void TimeEn(params object[] stuff);
        void Trace(params object[] stuff);
        void Warn(params object[] stuff);
    }
}
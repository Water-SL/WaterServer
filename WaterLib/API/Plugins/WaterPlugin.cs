namespace Water
{
    public abstract class WaterPlugin : IWaterPlugin
    {
        public abstract string Name { get; }

        public virtual string Author
        {
            get { return ""; }
        }

        public void Enable()
        {
            OnEnabled();
        }

        public void Disable()
        {
            OnDisabled();
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }

        protected void Log(string message)
        {
            WaterApi.WriteLog("[" + Name + "] " + (message ?? ""));
        }

        protected void RegisterCommand(string name, string summary, System.Action<string> handler)
        {
            CommandBus.Register(name, summary, handler);
        }
    }
}

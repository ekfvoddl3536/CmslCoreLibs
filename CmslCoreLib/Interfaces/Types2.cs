namespace CmslCore
{
    public interface ISupportSafeLoad
    {
        bool SafeLoad();

        void Initialize();
    }

    public interface ISupportInt32Tag
    {
        int Int32Tag { get; set; }
    }
}

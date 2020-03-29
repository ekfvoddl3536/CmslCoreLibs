using SuperComicLib.IO;

namespace CmslCore
{
    public interface ICmslServer
    {

        string GetCorePath { get; }

        string GetJavaPath { get; }

        string GetJVMArgs { get; }

        int GetMaxinumSize { get; }

        int GetMinimumSize { get; }

        bool IsCreated { get; }

        bool IsNullArgs { get; }

        bool IsRunning { get; }

        bool Abort();

        void BeginReadCommand();

        void BeginReadCommand(uint max);

        void CancelReadCommand();

        void ResizeMemoryMB(int min, int max);

        void ResizeMemoryGB(short min, short max);

        bool Start(string folder);

        void StdInput(string text);
    }

    public interface ICloneable<T>
    {
        T Clone();
    }

    public interface ISaveLoadable
    {
        void Serialize(FastWriter fs);
        void Deserialize(FastReader fs);
    }
}

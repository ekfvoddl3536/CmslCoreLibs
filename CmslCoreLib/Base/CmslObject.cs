using SuperComicLib.IO;
using System;

namespace CmslCore
{
    public abstract class CmslObject : ICloneable<CmslObject>, ISaveLoadable, IDisposable
    {
        public abstract CmslObject Clone();

        public abstract void Serialize(FastWriter fs);

        public abstract void Deserialize(FastReader fs);

        ~CmslObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }
    }
}

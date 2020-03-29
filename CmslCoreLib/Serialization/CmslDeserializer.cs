using SuperComicLib.IO;
using System.IO;
using System.Text;

namespace CmslCore
{
    public class CmslDeserializer : FastReader
    {
        public CmslDeserializer(Stream baseStream) : base(baseStream)
        {
        }

        public CmslDeserializer(string filepath) : base(filepath)
        {
        }

        protected CmslDeserializer()
        {
        }

        public override string ReadString()
        {
            int len = ReadInt32();
            return len < 0 ? null : Encoding.UTF8.GetString(ReadBytes(len));
        }
    }
}

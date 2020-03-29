using SuperComicLib.IO;
using System.IO;
using System.Text;

namespace CmslCore
{
    public class CmslSerializer : FastWriter
    {
        public CmslSerializer(Stream baseStream) : base(baseStream)
        {
        }

        public CmslSerializer(string filepath) : base(filepath)
        {
        }

        public CmslSerializer(string filepath, FileMode mode, FileAccess access) : base(filepath, mode, access)
        {
        }

        protected CmslSerializer()
        {
        }

        public override void Write(string strdata)
        {
            if (string.IsNullOrEmpty(strdata))
                Write(-1);
            else
            {
                byte[] vs = Encoding.UTF8.GetBytes(strdata);
                Write(vs.Length);
                Write(vs);
            }
        }
    }
}

using SuperComicLib.IO;
using System;

namespace CmslCore
{
    public class AdvanceCmslServer : CmslServer, ISaveLoadable
    {
        public event Action<AdvanceCmslServer> OnStarting;
        public event Action<AdvanceCmslServer> OnStoping;

        public AdvanceCmslServer()
        {
        }

        public AdvanceCmslServer(string jvm_args) : base(jvm_args)
        {
        }

        public override bool Start(string folder)
        {
            OnStarting?.Invoke(this);
            return base.Start(folder);
        }

        protected override void Process_Exited(object sender, EventArgs e)
        {
            OnStoping?.Invoke(this);
            base.Process_Exited(sender, e);
        }

        public virtual void Serialize(FastWriter fs)
        {
            fs.Write(m_miniRam);
            fs.Write(m_maxiRam);
            fs.Write(javapath ?? string.Empty);
            fs.Write(corepath ?? string.Empty);
            fs.Write(jvm_line ?? string.Empty);
        }

        public virtual void Deserialize(FastReader fs)
        {
            m_miniRam = fs.ReadInt32();
            m_maxiRam = fs.ReadInt32();
            javapath = fs.ReadString();
            corepath = fs.ReadString();
            jvm_line = fs.ReadString();
        }
    }
}

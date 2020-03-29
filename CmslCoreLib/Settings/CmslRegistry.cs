using System;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace CmslCore
{
    public static class CmslRegistry
    {
        public const string software = "Software";
        public const string def_subkeyname = "CmslQ2_0";
        public const string tree_subkey = software + "\\" + def_subkeyname;

        private static RegistryKey reg;

        public static void Initialize()
        {
            if (reg != null)
                return;

            RegistrySecurity rs = new RegistrySecurity();
            rs.AddAccessRule(new RegistryAccessRule(Environment.UserDomainName + "\\" + Environment.UserName,
                RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
            reg = Registry.CurrentUser.CreateSubKey("Software", RegistryKeyPermissionCheck.Default, rs).CreateSubKey(def_subkeyname);
        }

        public static RegistryKey AddOrGetSubKey(string name) => reg.CreateSubKey(name);

        public static void ClearTree()
        {
            if (IsInitialized && !IsEmptyTree)
                Registry.LocalMachine.DeleteSubKey(tree_subkey);
        }

        public static void SetValue(string name, int value)
        {
            reg.SetValue(name, value, RegistryValueKind.DWord);
            reg.Flush();
        }

        public static void SetValue(string name, uint value)
        {
            reg.SetValue(name, value, RegistryValueKind.DWord);
            reg.Flush();
        }

        public static void SetValue(string name, long value)
        {
            reg.SetValue(name, value, RegistryValueKind.QWord);
            reg.Flush();
        }

        public static void SetValue(string name, ulong value)
        {
            reg.SetValue(name, value, RegistryValueKind.QWord);
            reg.Flush();
        }

        public static void SetValue(string name, bool value) => SetValue(name, value ? 1 : 0);

        public static void SetValue(string name, string value)
        {
            reg.SetValue(name, value ?? string.Empty, RegistryValueKind.String);
            reg.Flush();
        }

        public static string GetValue(string name, string def) => reg.GetValue(name, def).ToString();

        public static int GetValue(string name, int def) => Convert.ToInt32(reg.GetValue(name, def));

        public static uint GetValue(string name, uint def) => Convert.ToUInt32(reg.GetValue(name, def));

        public static long GetValue(string name, long def) => Convert.ToInt64(reg.GetValue(name, def));

        public static ulong GetValue(string name, ulong def) => Convert.ToUInt32(reg.GetValue(name, def));

        public static bool GetValue(string name, bool def) => GetValue(name, def ? 1 : 0) > 0;

        public static bool IsNullKey(string name) => reg.GetValue(name) == null;

        public static bool IsInitialized => reg != null;

        public static bool IsEmptyTree => reg.ValueCount == 0;
    }
}

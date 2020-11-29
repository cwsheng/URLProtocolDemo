using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace URLProtocolDemo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            RegisterUrlProtocol();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var from = new Form1();
            from.Args = args;
            Application.Run(from);
        }


        /// <summary>
        /// 注册自定义协议
        /// </summary>
        private static void RegisterUrlProtocol()
        {
            try
            {
                //检查是否注册自定义协议：如未注册则注册
                Register register = new Register("calldemo", RegDomain.ClassesRoot);
                if (!register.IsSubKeyExist("calldemo"))
                {
                    //注册：
                    register.CreateSubKey();
                    register.WriteRegeditKey("", $"{Application.ExecutablePath}");
                    register.WriteRegeditKey("URL Protocol", "");
                    if (!register.IsSubKeyExist(@"calldemo\DefaultIcon"))
                    {
                        register.CreateSubKey(@"calldemo\DefaultIcon");
                        register.SubKey = @"calldemo\DefaultIcon";
                        register.WriteRegeditKey("", $"{Application.ExecutablePath},1");
                    }

                    if (!register.IsSubKeyExist(@"calldemo\shell"))
                    {
                        register.CreateSubKey(@"calldemo\shell");
                        register.CreateSubKey(@"calldemo\shell\open");
                        register.CreateSubKey(@"calldemo\shell\open\command");
                        register.SubKey = @"calldemo\shell\open\command";
                        register.WriteRegeditKey("", $"{Application.ExecutablePath} %1");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }
    }
}

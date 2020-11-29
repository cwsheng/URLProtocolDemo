using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            Process instance = RunningInstance();
            if (instance == null)
            {
                //1.1 没有实例在运行
                var from = new Form1();
                //显示输入参数
                from.Args = args;
                Application.Run(from);
            }
            else
            {
                //1.2 已经有一个实例在运行
                HandleRunningInstance(instance);
            }
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
                        //添加默认键
                        register.WriteRegeditKey("", $"\"{Application.ExecutablePath}\" \"%1\"");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }


        #region 确保程序只运行一个实例
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表 
            foreach (Process process in processes)
            {
                //如果实例已经存在则忽略当前进程 
                if (process.Id != current.Id)
                {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //返回已经存在的进程
                        return process;
                    }
                }
            }
            return null;
        }
        //3.已经有了就把它激活，并将其窗口放置最前端
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, 1); //调用api函数，正常显示窗口
            SetForegroundWindow(instance.MainWindowHandle); //将窗口放置最前端
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(System.IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(System.IntPtr hWnd);
        #endregion
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using TrOCR.Helper;

namespace TrOCR
{

    internal static class Program
    {
        public static EventWaitHandle ProgramStarted;

        public static float factor;

        public static bool createNew;

        public static System.Timers.Timer checkTimer;

        public static void ProgramStart()
        {
            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "天若OCR文字识别", out createNew);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            SetConfig();
            bool_error();
            checkTimer = new System.Timers.Timer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var version = Environment.OSVersion.Version;
            var value = new Version("6.1");
            factor = GetDpi_factor();
            if (version.CompareTo(value) >= 0)
            {
                SetProcessDPIAware();
            }

            ProgramStart();
            if (!createNew)
            {
                ProgramStarted.Set();
                var fmFlags = new FmFlags();
                fmFlags.Show();
                fmFlags.DrawStr("软件已经运行");
                return;
            }

            if (args.Length != 0 && args[0] == "更新")
            {
                new FmSetting
                {
                    Start_set = ""
                }.ShowDialog();
            }

            if (IniHelp.GetValue("更新", "检测更新") == "True" || IniHelp.GetValue("更新", "检测更新") == "发生错误")
            {
                new Thread(CheckUpdate).Start();
                if (IniHelp.GetValue("更新", "更新间隔") == "True")
                {
                    checkTimer.Enabled = true;
                    checkTimer.Interval = 3600000.0 * Convert.ToInt32(IniHelp.GetValue("更新", "间隔时间"));
                    checkTimer.Elapsed += CheckTimer_Elapsed;
                    checkTimer.Start();
                }
            }
            else
            {
                var fmflags2 = new FmFlags();
                fmflags2.Show();
                fmflags2.DrawStr("天若OCR文字识别");
            }

            Application.Run(new FmMain());
        }

        public static void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        [DllImport("wininet")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        public static bool IsConnectedInternet()
        {
            var num = 0;
            return InternetGetConnectedState(out num, 0);
        }

        public static int GetPidByProcessName(string processName)
        {
            var processesByName = Process.GetProcessesByName(processName);
            var num = 0;
            var flag = num >= processesByName.Length;
            var flag2 = flag;
            var flag3 = flag2;
            var flag4 = flag3;
            var flag5 = flag4;
            var flag6 = flag5;
            int result;
            if (flag6)
            {
                result = 0;
            }
            else
            {
                result = processesByName[num].Id;
            }

            return result;
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public static float GetDpi_factor()
        {
            float result;
            try
            {
                var name = "AppliedDPI";
                var registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\WindowMetrics", true);
                var value = registryKey.GetValue(name).ToString();
                registryKey.Close();
                result = Convert.ToSingle(value) / 96f;
            }
            catch
            {
                result = 1f;
            }

            return result;
        }

        public static void CheckUpdate()
        {
            var html = WebHelper.GetHtmlContent("https://www.jianshu.com/p/3afe79471cb9");
            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            var regex = Regex.Match(html, @"(?<=<pre><code>)[\s\S]+?(?=</code>)");
            if (regex.Success)
            {
                var code = regex.Value.Trim();
                var json = JObject.Parse(code);
                var newVersion = json["version"].Value<string>();
                var curVersion = Application.ProductVersion;
                if (!CheckVersion(newVersion, curVersion))
                {
                    var ff = new FmFlags();
                    ff.Show();
                    ff.DrawStr("当前已是最新版本");
                    return;
                }

                var flagForm = new FmFlags();
                flagForm.Show();
                flagForm.DrawStr("有新版本：" + newVersion);
                var fullUpdate = json["full_update"].Value<bool>();
                if (fullUpdate)
                {
                    MessageBox.Show($"发现新版本：{newVersion}，请到百度网盘下载！", "提醒");
                    Process.Start("https://pan.baidu.com/s/1P2xb9kBwX1gj8j2_APivZw");
                }
                else
                {
                    Process.Start("Data\\update.exe", " " + json["main_url"].Value<string>() + " " +
                                                      Path.Combine(Application.ExecutablePath, "天若OCR文字识别.exe"));
                    Environment.Exit(0);
                }
            }
        }

        private static bool CheckVersion(string newVersion, string curVersion)
        {
            var arr1 = newVersion.Split('.');
            var arr2 = curVersion.Split('.');
            for (int i = 0; i < arr1.Length; i++)
            {
                if (Convert.ToInt32(arr1[i]) > Convert.ToInt32(arr2[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static void SetConfig()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "Data\\config.ini";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data");
            }

            if (!File.Exists(path))
            {
                using (File.Create(path))
                {
                }

                IniHelp.SetValue("配置", "接口", "搜狗");
                IniHelp.SetValue("配置", "开机自启", "True");
                IniHelp.SetValue("配置", "快速翻译", "True");
                IniHelp.SetValue("配置", "识别弹窗", "True");
                IniHelp.SetValue("配置", "窗体动画", "窗体");
                IniHelp.SetValue("配置", "记录数目", "20");
                IniHelp.SetValue("配置", "自动保存", "True");
                IniHelp.SetValue("配置", "截图位置", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                IniHelp.SetValue("配置", "翻译接口", "谷歌");
                IniHelp.SetValue("快捷键", "文字识别", "F4");
                IniHelp.SetValue("快捷键", "翻译文本", "F9");
                IniHelp.SetValue("快捷键", "记录界面", "请按下快捷键");
                IniHelp.SetValue("快捷键", "识别界面", "请按下快捷键");
                IniHelp.SetValue("密钥_百度", "secret_id", "YsZKG1wha34PlDOPYaIrIIKO");
                IniHelp.SetValue("密钥_百度", "secret_key", "HPRZtdOHrdnnETVsZM2Nx7vbDkMfxrkD");
                IniHelp.SetValue("代理", "代理类型", "系统代理");
                IniHelp.SetValue("代理", "服务器", "");
                IniHelp.SetValue("代理", "端口", "");
                IniHelp.SetValue("代理", "需要密码", "False");
                IniHelp.SetValue("代理", "服务器账号", "");
                IniHelp.SetValue("代理", "服务器密码", "");
                IniHelp.SetValue("更新", "检测更新", "True");
                IniHelp.SetValue("更新", "更新间隔", "True");
                IniHelp.SetValue("更新", "间隔时间", "24");
                IniHelp.SetValue("截图音效", "自动保存", "True");
                IniHelp.SetValue("截图音效", "音效路径", "Data\\screenshot.wav");
                IniHelp.SetValue("截图音效", "粘贴板", "False");
                IniHelp.SetValue("工具栏", "合并", "False");
                IniHelp.SetValue("工具栏", "分段", "False");
                IniHelp.SetValue("工具栏", "分栏", "False");
                IniHelp.SetValue("工具栏", "拆分", "False");
                IniHelp.SetValue("工具栏", "检查", "False");
                IniHelp.SetValue("工具栏", "翻译", "False");
                IniHelp.SetValue("工具栏", "顶置", "True");
                IniHelp.SetValue("取色器", "类型", "RGB");
            }
        }

        public static void bool_error()
        {
            if (IniHelp.GetValue("配置", "接口") == "发生错误")
            {
                IniHelp.SetValue("配置", "接口", "搜狗");
            }

            if (IniHelp.GetValue("配置", "开机自启") == "发生错误")
            {
                IniHelp.SetValue("配置", "开机自启", "True");
            }

            if (IniHelp.GetValue("配置", "快速翻译") == "发生错误")
            {
                IniHelp.SetValue("配置", "快速翻译", "True");
            }

            if (IniHelp.GetValue("配置", "识别弹窗") == "发生错误")
            {
                IniHelp.SetValue("配置", "识别弹窗", "True");
            }

            if (IniHelp.GetValue("配置", "窗体动画") == "发生错误")
            {
                IniHelp.SetValue("配置", "窗体动画", "窗体");
            }

            if (IniHelp.GetValue("配置", "记录数目") == "发生错误")
            {
                IniHelp.SetValue("配置", "记录数目", "20");
            }

            if (IniHelp.GetValue("配置", "自动保存") == "发生错误")
            {
                IniHelp.SetValue("配置", "自动保存", "True");
            }

            if (IniHelp.GetValue("配置", "翻译接口") == "发生错误")
            {
                IniHelp.SetValue("配置", "翻译接口", "谷歌");
            }

            if (IniHelp.GetValue("配置", "截图位置") == "发生错误")
            {
                IniHelp.SetValue("配置", "截图位置", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            }

            if (IniHelp.GetValue("快捷键", "文字识别") == "发生错误")
            {
                IniHelp.SetValue("快捷键", "文字识别", "F4");
            }

            if (IniHelp.GetValue("快捷键", "翻译文本") == "发生错误")
            {
                IniHelp.SetValue("快捷键", "翻译文本", "F9");
            }

            if (IniHelp.GetValue("快捷键", "记录界面") == "发生错误")
            {
                IniHelp.SetValue("快捷键", "记录界面", "请按下快捷键");
            }

            if (IniHelp.GetValue("快捷键", "识别界面") == "发生错误")
            {
                IniHelp.SetValue("快捷键", "识别界面", "请按下快捷键");
            }

            if (IniHelp.GetValue("密钥_百度", "secret_id") == "发生错误")
            {
                IniHelp.SetValue("密钥_百度", "secret_id", "YsZKG1wha34PlDOPYaIrIIKO");
            }

            if (IniHelp.GetValue("密钥_百度", "secret_key") == "发生错误")
            {
                IniHelp.SetValue("密钥_百度", "secret_key", "HPRZtdOHrdnnETVsZM2Nx7vbDkMfxrkD");
            }

            if (IniHelp.GetValue("代理", "代理类型") == "发生错误")
            {
                IniHelp.SetValue("代理", "代理类型", "系统代理");
            }

            if (IniHelp.GetValue("代理", "服务器") == "发生错误")
            {
                IniHelp.SetValue("代理", "服务器", "");
            }

            if (IniHelp.GetValue("代理", "端口") == "发生错误")
            {
                IniHelp.SetValue("代理", "端口", "");
            }

            if (IniHelp.GetValue("代理", "需要密码") == "发生错误")
            {
                IniHelp.SetValue("代理", "需要密码", "False");
            }

            if (IniHelp.GetValue("代理", "服务器账号") == "发生错误")
            {
                IniHelp.SetValue("代理", "服务器账号", "");
            }

            if (IniHelp.GetValue("代理", "服务器密码") == "发生错误")
            {
                IniHelp.SetValue("代理", "服务器密码", "");
            }

            if (IniHelp.GetValue("更新", "检测更新") == "发生错误")
            {
                IniHelp.SetValue("更新", "检测更新", "True");
            }

            if (IniHelp.GetValue("更新", "更新间隔") == "发生错误")
            {
                IniHelp.SetValue("更新", "更新间隔", "True");
            }

            if (IniHelp.GetValue("更新", "间隔时间") == "发生错误")
            {
                IniHelp.SetValue("更新", "间隔时间", "24");
            }

            if (IniHelp.GetValue("截图音效", "自动保存") == "发生错误")
            {
                IniHelp.SetValue("截图音效", "自动保存", "True");
            }

            if (IniHelp.GetValue("截图音效", "音效路径") == "发生错误")
            {
                IniHelp.SetValue("截图音效", "音效路径", "Data\\screenshot.wav");
            }

            if (IniHelp.GetValue("截图音效", "粘贴板") == "发生错误")
            {
                IniHelp.SetValue("截图音效", "粘贴板", "False");
            }

            if (IniHelp.GetValue("工具栏", "合并") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "合并", "False");
            }

            if (IniHelp.GetValue("工具栏", "拆分") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "拆分", "False");
            }

            if (IniHelp.GetValue("工具栏", "检查") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "检查", "False");
            }

            if (IniHelp.GetValue("工具栏", "翻译") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "翻译", "False");
            }

            if (IniHelp.GetValue("工具栏", "分段") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "分段", "False");
            }

            if (IniHelp.GetValue("工具栏", "分栏") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "分栏", "False");
            }

            if (IniHelp.GetValue("工具栏", "顶置") == "发生错误")
            {
                IniHelp.SetValue("工具栏", "顶置", "True");
            }

            if (IniHelp.GetValue("取色器", "类型") == "发生错误")
            {
                IniHelp.SetValue("取色器", "类型", "RGB");
            }

            if (IniHelp.GetValue("特殊", "ali_cookie") == "发生错误")
            {
                IniHelp.SetValue("特殊", "ali_cookie",
                    "cna=noXhE38FHGkCAXDve7YaZ8Tn; cnz=noXhE4/VhB8CAbZ773ApeV14; isg=BGJi2c2YTeeP6FG7S_Re8kveu-jEs2bNwToQnKz7jlWAfwL5lEO23eh9q3km9N5l; ");
            }

            if (IniHelp.GetValue("特殊", "ali_account") == "发生错误")
            {
                IniHelp.SetValue("特殊", "ali_account", "");
            }

            if (IniHelp.GetValue("特殊", "ali_password") == "发生错误")
            {
                IniHelp.SetValue("特殊", "ali_password", "");
            }
        }
    }
}

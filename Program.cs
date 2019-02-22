using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TrOCR
{

	internal static class Program
	{

		//private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		//{
		//	var executingAssembly = Assembly.GetExecutingAssembly();
		//	var name = MethodBase.GetCurrentMethod().DeclaringType.Namespace + "." + new AssemblyName(args.Name).Name + ".dll";
		//	Assembly result;
		//	using (var manifestResourceStream = executingAssembly.GetManifestResourceStream(name))
		//	{
		//		var array = new byte[manifestResourceStream.Length];
		//		manifestResourceStream.Read(array, 0, array.Length);
		//		result = Assembly.Load(array);
		//	}
		//	return result;
		//}

		public static void ProgramStart()
		{
			ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, "天若OCR文字识别",out createNew);
		}

		public static void Get_version(string url)
		{
			try
			{
				string input;
				var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
				httpWebRequest.Method = "GET";
				using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						input = streamReader.ReadToEnd();
						streamReader.Close();
						httpWebResponse.Close();
					}
				}
				httpWebRequest.Abort();
				httpWebRequest = null;
				var text = "<li";
				var text2 = "</ul></li>";
				var text3 = "id=\"ml_";
				var text4 = "\"";
				var text5 = "";
				var matchCollection = Regex.Matches(input, string.Concat("(?<=(", text, "))[.\\s\\S]*?(?=(", text2, "))"));
				for (var i = 0; i < matchCollection.Count; i++)
				{
					var flag = matchCollection[i].Value.Contains(StaticValue.content);
					var flag2 = flag;
					var flag3 = flag2;
					var flag4 = flag3;
					var flag5 = flag4;
					var flag6 = flag5;
					if (flag6)
					{
						text5 = new Regex(string.Concat("(?<=(", text3, "))[.\\s\\S]*?(?=(", text4, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(matchCollection[i].Value).Value;
						break;
					}
				}
				var text6 = Get_html("http://cc.ys168.com/f_ht/ajcx/wj.aspx?cz=dq&mlbh=" + text5 + "&_dlmc=tianruoyouxin&_dlmm=");
				var text7 = "版本V";
				var text8 = "<";
				var flag7 = text6 == "";
				var flag8 = flag7;
				var flag9 = flag8;
				var flag10 = flag9;
				var flag11 = flag10;
				var flag12 = flag11;
				if (flag12)
				{
					version_url = "0";
					download_url = "0";
				}
				else
				{
					var flag13 = text5 == "";
					var flag14 = flag13;
					var flag15 = flag14;
					var flag16 = flag15;
					var flag17 = flag16;
					var flag18 = flag17;
					if (flag18)
					{
						version_url = "0";
						download_url = "0";
					}
					else
					{
						version_url = new Regex(string.Concat("(?<=(", text7, "))[.\\s\\S]*?(?=(", text8, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
						var text9 = "url=\"";
						var text10 = "\"";
						download_url = new Regex(string.Concat("(?<=(", text9, "))[.\\s\\S]*?(?=(", text10, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
					}
				}
			}
			catch
			{
				version_url = "0";
				download_url = "0";
			}
		}

		[STAThread]
		public static void Main(string[] args)
		{
			setini();
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
			//try
			//{
			//	AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			//}
			//catch
			//{
			//	MessageBox.Show("调用dll出错！");
			//}
			if (args.Length != 0 && args[0] == "更新")
			{
				new FmSetting
				{
					Start_set = ""
				}.ShowDialog();
			}
			if (IniHelp.GetValue("更新", "检测更新") == "True" || IniHelp.GetValue("更新", "检测更新") == "发生错误")
			{
				//MessageBox.Show("开放全新的版本，有兴趣的想办法联系我，受制于吾爱规矩该软件不留联系方式。\r\n若不愿使用，请在设置中关闭检测更新！\r\n该版本不再进行维护！", "提醒");
				new Thread(new ThreadStart(check_update)).Start();
				if (IniHelp.GetValue("更新", "更新间隔") == "True")
				{
					checkTimer.Enabled = true;
					checkTimer.Interval = 3600000.0 * (double)Convert.ToInt32(IniHelp.GetValue("更新", "间隔时间"));
					checkTimer.Elapsed += checkTimer_Elapsed;
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

		public static void DeleteFile(string path)
		{
			var messageload = new Messageload();
			messageload.ShowDialog();
			var dialogResult = messageload.DialogResult;
			var flag = File.GetAttributes(path) == FileAttributes.Directory;
			var flag2 = flag;
			var flag3 = flag2;
			var flag4 = flag3;
			var flag5 = flag4;
			var flag6 = flag5;
			if (flag6)
			{
				Directory.Delete(path, true);
			}
			else
			{
				File.Delete(path);
			}
		}

		public static string Get_html(string url)
		{
			var result = "";
			var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers.Add("Accept-Language:zh-CN,zh;q=0.8");
			try
			{
				using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						result = streamReader.ReadToEnd();
						streamReader.Close();
						httpWebResponse.Close();
					}
				}
				httpWebRequest.Abort();
			}
			catch
			{
				result = "";
			}
			return result;
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

		public static void check_update()
		{
			Get_update("http://cc.ys168.com/f_ht/ajcx/ml.aspx?cz=ml_dq&_dlmc=tianruoyouxin&_dlmm=");
		}

		public static void Get_update(string url)
		{
			var fmflags = new FmFlags();
			fmflags.Show();
			fmflags.DrawStr("天若OCR文字识别");
			try
			{
				var input = "";
				var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
				httpWebRequest.Method = "GET";
				using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						input = streamReader.ReadToEnd();
						streamReader.Close();
						httpWebResponse.Close();
					}
				}
				httpWebRequest.Abort();
				httpWebRequest = null;
				var text = "<li";
				var text2 = "</ul></li>";
				var text3 = "id=\"ml_";
				var text4 = "\"";
				var text5 = "";
				var matchCollection = Regex.Matches(input, string.Concat("(?<=(", text, "))[.\\s\\S]*?(?=(", text2, "))"));
				for (var i = 0; i < matchCollection.Count; i++)
				{
					if (matchCollection[i].Value.Contains(StaticValue.content))
					{
						text5 = new Regex(string.Concat("(?<=(", text3, "))[.\\s\\S]*?(?=(", text4, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(matchCollection[i].Value).Value;
						break;
					}
				}
				var text6 = Get_html("http://cc.ys168.com/f_ht/ajcx/wj.aspx?cz=dq&mlbh=" + text5 + "&_dlmc=tianruoyouxin&_dlmm=");
				var text7 = "版本V";
				var text8 = "<";
				if (text6 == "")
				{
					version_url = "0";
					download_url = "0";
				}
				else if (text5 == "")
				{
					version_url = "0";
					download_url = "0";
				}
				else
				{
					version_url = new Regex(string.Concat("(?<=(", text7, "))[.\\s\\S]*?(?=(", text8, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
					var text9 = "url=\"";
					var text10 = "\"";
					download_url = new Regex(string.Concat("(?<=(", text9, "))[.\\s\\S]*?(?=(", text10, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
				}
			}
			catch
			{
				version_url = "0";
				download_url = "0";
			}
			if (version_url != "0" && Convert.ToSingle(version_url) > Convert.ToSingle(StaticValue.current_v))
			{
				fmflags.Show();
				fmflags.DrawStr("有新版本V" + version_url);
			}
		}

		private static void DeleteItself()
		{
			var text = Path.GetDirectoryName(Application.ExecutablePath) + "\\DeleteItself.bat";
			using (var streamWriter = new StreamWriter(text, false, Encoding.Default))
			{
				streamWriter.Write(string.Format(":del\r\n del \"{0}\"\r\nif exist \"{0}\" goto del\r\ndel %0\r\n", Application.ExecutablePath.Replace("/", "\\")));
			}
			HelpWin32.WinExec(text, 0u);
			Application.Exit();
		}

        public static void Get_update_x(string url)
		{
			try
			{
				var input = "";
				var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
				httpWebRequest.Method = "GET";
				using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						input = streamReader.ReadToEnd();
						streamReader.Close();
						httpWebResponse.Close();
					}
				}
				httpWebRequest.Abort();
				httpWebRequest = null;
				var text = "<li";
				var text2 = "</ul></li>";
				var text3 = "id=\"ml_";
				var text4 = "\"";
				var text5 = "";
				var matchCollection = Regex.Matches(input, string.Concat("(?<=(", text, "))[.\\s\\S]*?(?=(", text2, "))"));
				for (var i = 0; i < matchCollection.Count; i++)
				{
					var flag = matchCollection[i].Value.Contains(StaticValue.content);
					var flag2 = flag;
					var flag3 = flag2;
					var flag4 = flag3;
					var flag5 = flag4;
					var flag6 = flag5;
					if (flag6)
					{
						text5 = new Regex(string.Concat("(?<=(", text3, "))[.\\s\\S]*?(?=(", text4, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(matchCollection[i].Value).Value;
						break;
					}
				}
				var text6 = Get_html("http://cc.ys168.com/f_ht/ajcx/wj.aspx?cz=dq&mlbh=" + text5 + "&_dlmc=tianruoyouxin&_dlmm=");
				var text7 = "版本V";
				var text8 = "<";
				var flag7 = text6 == "";
				var flag8 = flag7;
				var flag9 = flag8;
				var flag10 = flag9;
				var flag11 = flag10;
				var flag12 = flag11;
				if (flag12)
				{
					version_url = "0";
					download_url = "0";
				}
				else
				{
					var flag13 = text5 == "";
					var flag14 = flag13;
					var flag15 = flag14;
					var flag16 = flag15;
					var flag17 = flag16;
					var flag18 = flag17;
					if (flag18)
					{
						version_url = "0";
						download_url = "0";
					}
					else
					{
						version_url = new Regex(string.Concat("(?<=(", text7, "))[.\\s\\S]*?(?=(", text8, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
						var text9 = "url=\"";
						var text10 = "\"";
						download_url = new Regex(string.Concat("(?<=(", text9, "))[.\\s\\S]*?(?=(", text10, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
					}
				}
			}
			catch
			{
				version_url = "0";
				download_url = "0";
			}
		}

		public static void checkTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			Get_update_check("http://cc.ys168.com/f_ht/ajcx/ml.aspx?cz=ml_dq&_dlmc=tianruoyouxin&_dlmm=");
		}

		public static void Get_update_check(string url)
		{
			try
			{
				var input = "";
				var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
				httpWebRequest.Method = "GET";
				using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
					{
						input = streamReader.ReadToEnd();
						streamReader.Close();
						httpWebResponse.Close();
					}
				}
				httpWebRequest.Abort();
				httpWebRequest = null;
				var text = "<li";
				var text2 = "</ul></li>";
				var text3 = "id=\"ml_";
				var text4 = "\"";
				var text5 = "";
				var matchCollection = Regex.Matches(input, string.Concat("(?<=(", text, "))[.\\s\\S]*?(?=(", text2, "))"));
				for (var i = 0; i < matchCollection.Count; i++)
				{
					var flag = matchCollection[i].Value.Contains(StaticValue.content);
					var flag2 = flag;
					var flag3 = flag2;
					if (flag3)
					{
						text5 = new Regex(string.Concat("(?<=(", text3, "))[.\\s\\S]*?(?=(", text4, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(matchCollection[i].Value).Value;
						break;
					}
				}
				var text6 = Get_html("http://cc.ys168.com/f_ht/ajcx/wj.aspx?cz=dq&mlbh=" + text5 + "&_dlmc=tianruoyouxin&_dlmm=");
				var text7 = "版本V";
				var text8 = "<";
				var flag4 = text6 != "" || text5 != "";
				var flag5 = flag4;
				var flag6 = flag5;
				if (flag6)
				{
					version_url = new Regex(string.Concat("(?<=(", text7, "))[.\\s\\S]*?(?=(", text8, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
					var text9 = "url=\"";
					var text10 = "\"";
					download_url = new Regex(string.Concat("(?<=(", text9, "))[.\\s\\S]*?(?=(", text10, "))"), RegexOptions.Multiline | RegexOptions.Singleline).Match(text6).Value;
				}
				else
				{
					version_url = "0";
					download_url = "0";
				}
			}
			catch
			{
				version_url = "0";
				download_url = "0";
			}
			var flag7 = !(version_url != "0") || Convert.ToSingle(version_url) <= Convert.ToSingle(StaticValue.current_v);
			var flag8 = flag7;
			var flag9 = flag8;
			if (flag9)
			{
				var fmflags = new FmFlags();
				fmflags.Show();
				fmflags.DrawStr("当前已是最新版本");
			}
			else
			{
				var fmflags2 = new FmFlags();
				fmflags2.Show();
				fmflags2.DrawStr("有新版本V" + version_url);
				Get_update_x("http://cc.ys168.com/f_ht/ajcx/ml.aspx?cz=ml_dq&_dlmc=tianruoyouxin&_dlmm=");
				var flag10 = version_url == "0";
				var flag11 = flag10;
				var flag12 = flag11;
				if (flag12)
				{
					MessageBox.Show("更新程序失效，请到百度网盘下载！", "提醒");
					Process.Start("https://pan.baidu.com/s/1P2xb9kBwX1gj8j2_APivZw");
				}
				else
				{
					var flag13 = version_url != "0";
					var flag14 = flag13;
					var flag15 = flag14;
					if (flag15)
					{
						var flag16 = new Version(version_url) > new Version(StaticValue.current_v);
						var flag17 = flag16;
						var flag18 = flag17;
						if (flag18)
						{
							Process.Start("Data\\update.exe", " " + download_url + " " + Path.GetFileName(Application.ExecutablePath));
							Environment.Exit(0);
						}
						else
						{
							Process.Start("Data\\update.exe", " 最新版本");
						}
					}
				}
			}
		}

		public static void check_update_cc()
		{
			Get_update_check("http://cc.ys168.com/f_ht/ajcx/ml.aspx?cz=ml_dq&_dlmc=tianruoyouxin&_dlmm=");
		}

		public static void setini()
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

		public static byte[] StreamToBytes(Stream stream)
		{
			var array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return array;
		}

		public static void StreamToFile(Stream stream, string fileName)
		{
			var array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			var fileStream = new FileStream(fileName, FileMode.Create);
			var binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(array);
			binaryWriter.Close();
			fileStream.Close();
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
				IniHelp.SetValue("特殊", "ali_cookie", "cna=noXhE38FHGkCAXDve7YaZ8Tn; cnz=noXhE4/VhB8CAbZ773ApeV14; isg=BGJi2c2YTeeP6FG7S_Re8kveu-jEs2bNwToQnKz7jlWAfwL5lEO23eh9q3km9N5l; ");
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

		public static EventWaitHandle ProgramStarted;

		public static float factor;

		public static bool createNew;

		public static string download_url = "0";

		public static string version_url = "0";

		public static System.Timers.Timer checkTimer;

		public static bool inProgss;
	}
}

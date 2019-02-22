using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace TrOCR
{

	internal class WebBrowserHelper
	{

		public static IHTMLDocument3 GetDocumentFromWindow(IHTMLWindow2 htmlWindow)
		{
			if (htmlWindow != null)
			{
				try
				{
					return (IHTMLDocument3)htmlWindow.document;
				}
				catch (COMException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (Exception)
				{
					return null;
				}
				try
				{
					var serviceProvider = (IServiceProvider)htmlWindow;
					object obj = null;
					serviceProvider.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out obj);
					return (IHTMLDocument3)((IWebBrowser2)obj).Document;
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
				return null;
			}
			return null;
		}

		private static Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

		private static Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E");

		[ComVisible(true)]
		[Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		public interface IServiceProvider
		{

			[PreserveSig]
			[return: MarshalAs(UnmanagedType.I4)]
			int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		}

		[Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E")]
		[TypeLibType(TypeLibTypeFlags.FHidden | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FOleAutomation)]
		[ComImport]
		public interface IWebBrowser2
		{

			[DispId(100)]
			void GoBack();

			[DispId(101)]
			void GoForward();

			[DispId(102)]
			void GoHome();

			[DispId(103)]
			void GoSearch();

			[DispId(104)]
			void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);

			[DispId(-550)]
			void Refresh();

			[DispId(105)]
			void Refresh2([In] ref object level);

			[DispId(106)]
			void Stop();

			// (get) Token: 0x060002C9 RID: 713
			[DispId(200)]
			object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

			// (get) Token: 0x060002CA RID: 714
			[DispId(201)]
			object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

			// (get) Token: 0x060002CB RID: 715
			[DispId(202)]
			object Container { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

			// (get) Token: 0x060002CC RID: 716
			[DispId(203)]
			object Document { [return: MarshalAs(UnmanagedType.IDispatch)] get; }

			// (get) Token: 0x060002CD RID: 717
			[DispId(204)]
			bool TopLevelContainer { get; }

			// (get) Token: 0x060002CE RID: 718
			[DispId(205)]
			string Type { get; }

			// (get) Token: 0x060002CF RID: 719
			// (set) Token: 0x060002D0 RID: 720
			[DispId(206)]
			int Left { get; set; }

			// (get) Token: 0x060002D1 RID: 721
			// (set) Token: 0x060002D2 RID: 722
			[DispId(207)]
			int Top { get; set; }

			// (get) Token: 0x060002D3 RID: 723
			// (set) Token: 0x060002D4 RID: 724
			[DispId(208)]
			int Width { get; set; }

			// (get) Token: 0x060002D5 RID: 725
			// (set) Token: 0x060002D6 RID: 726
			[DispId(209)]
			int Height { get; set; }

			// (get) Token: 0x060002D7 RID: 727
			[DispId(210)]
			string LocationName { get; }

			// (get) Token: 0x060002D8 RID: 728
			[DispId(211)]
			string LocationURL { get; }

			// (get) Token: 0x060002D9 RID: 729
			[DispId(212)]
			bool Busy { get; }

			[DispId(300)]
			void Quit();

			[DispId(301)]
			void ClientToWindow(out int pcx, out int pcy);

			[DispId(302)]
			void PutProperty([In] string property, [In] object vtValue);

			[DispId(303)]
			object GetProperty([In] string property);

			// (get) Token: 0x060002DE RID: 734
			[DispId(0)]
			string Name { get; }

			// (get) Token: 0x060002DF RID: 735
			[DispId(-515)]
			int HWND { get; }

			// (get) Token: 0x060002E0 RID: 736
			[DispId(400)]
			string FullName { get; }

			// (get) Token: 0x060002E1 RID: 737
			[DispId(401)]
			string Path { get; }

			// (get) Token: 0x060002E2 RID: 738
			// (set) Token: 0x060002E3 RID: 739
			[DispId(402)]
			bool Visible { get; set; }

			// (get) Token: 0x060002E4 RID: 740
			// (set) Token: 0x060002E5 RID: 741
			[DispId(403)]
			bool StatusBar { get; set; }

			// (get) Token: 0x060002E6 RID: 742
			// (set) Token: 0x060002E7 RID: 743
			[DispId(404)]
			string StatusText { get; set; }

			// (get) Token: 0x060002E8 RID: 744
			// (set) Token: 0x060002E9 RID: 745
			[DispId(405)]
			int ToolBar { get; set; }

			// (get) Token: 0x060002EA RID: 746
			// (set) Token: 0x060002EB RID: 747
			[DispId(406)]
			bool MenuBar { get; set; }

			// (get) Token: 0x060002EC RID: 748
			// (set) Token: 0x060002ED RID: 749
			[DispId(407)]
			bool FullScreen { get; set; }

			[DispId(500)]
			void Navigate2([In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);

			[DispId(501)]
			OLECMDF QueryStatusWB([In] OLECMDID cmdID);

			[DispId(502)]
			void ExecWB([In] OLECMDID cmdID, [In] OLECMDEXECOPT cmdexecopt, ref object pvaIn, IntPtr pvaOut);

			[DispId(503)]
			void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);

			// (get) Token: 0x060002F2 RID: 754
			[DispId(-525)]
			WebBrowserReadyState ReadyState { get; }

			// (get) Token: 0x060002F3 RID: 755
			// (set) Token: 0x060002F4 RID: 756
			[DispId(550)]
			bool Offline { get; set; }

			// (get) Token: 0x060002F5 RID: 757
			// (set) Token: 0x060002F6 RID: 758
			[DispId(551)]
			bool Silent { get; set; }

			// (get) Token: 0x060002F7 RID: 759
			// (set) Token: 0x060002F8 RID: 760
			[DispId(552)]
			bool RegisterAsBrowser { get; set; }

			// (get) Token: 0x060002F9 RID: 761
			// (set) Token: 0x060002FA RID: 762
			[DispId(553)]
			bool RegisterAsDropTarget { get; set; }

			// (get) Token: 0x060002FB RID: 763
			// (set) Token: 0x060002FC RID: 764
			[DispId(554)]
			bool TheaterMode { get; set; }

			// (get) Token: 0x060002FD RID: 765
			// (set) Token: 0x060002FE RID: 766
			[DispId(555)]
			bool AddressBar { get; set; }

			// (get) Token: 0x060002FF RID: 767
			// (set) Token: 0x06000300 RID: 768
			[DispId(556)]
			bool Resizable { get; set; }
		}

		public enum OLECMDEXECOPT
		{

			OLECMDEXECOPT_DODEFAULT,

			OLECMDEXECOPT_PROMPTUSER,

			OLECMDEXECOPT_DONTPROMPTUSER,

			OLECMDEXECOPT_SHOWHELP
		}

		public enum OLECMDF
		{

			OLECMDF_DEFHIDEONCTXTMENU = 32,

			OLECMDF_ENABLED = 2,

			OLECMDF_INVISIBLE = 16,

			OLECMDF_LATCHED = 4,

			OLECMDF_NINCHED = 8,

			OLECMDF_SUPPORTED = 1
		}

		public enum OLECMDID
		{

			OLECMDID_PAGESETUP = 8,

			OLECMDID_PRINT = 6,

			OLECMDID_PRINTPREVIEW,

			OLECMDID_PROPERTIES = 10,

			OLECMDID_SAVEAS = 4
		}
	}
}

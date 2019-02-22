using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TrOCR
{

	public class RichTextBoxEx : HelpRepaint.AdvRichTextBox
	{

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string path);

		// (get) Token: 0x060002BB RID: 699 RVA: 0x00002FD6 File Offset: 0x000011D6
		// (set) Token: 0x060002BC RID: 700 RVA: 0x00002FDE File Offset: 0x000011DE
		[Bindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[SettingsBindable(true)]
		[DefaultValue(false)]
		[Category("Appearance")]
		public string Rtf2
		{
			get
			{
				return Rtf;
			}
			set
			{
				Rtf = value;
			}
		}

		private IContainer components;

		private static IntPtr moduleHandle;
	}
}

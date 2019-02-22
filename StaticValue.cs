using System;
using System.Drawing;

namespace TrOCR
{

	public static class StaticValue
	{

		static StaticValue()
		{
			note = "";
			v_notecount = 40;
			copy_f = "无格式";
			content = "天若OCR更新";
			zh_en = true;
			zh_jp = false;
			zh_ko = false;
			set_默认 = true;
			set_拆分 = false;
			set_合并 = false;
			set_翻译 = false;
			set_记录 = false;
			set_截图 = false;
			Dpifactor = 1f;
			current_v = "5.0.0";
			v_date = "2019-02-22";
		}

		public static string v_Split;

		public static string v_Restore;

		public static string v_Merge;

		public static string v_googleTranslate_txt;

		public static string v_googleTranslate_back;

		public static int image_h;

		public static int image_w;

		public static string v_single;

		public static Image image_OCR;

		public static string current_v;

		public static string copy_f;

		public static string content;

		public static bool zh_en;

		public static bool zh_jp;

		public static bool zh_ko;

		public static bool set_默认;

		public static bool set_拆分;

		public static bool set_合并;

		public static bool set_翻译;

		public static bool set_记录;

		public static bool set_截图;

		public static float Dpifactor;

		public static IntPtr mainhandle;

		public static string note;

		public static string[] v_note;

		public static int v_notecount;

		public static string baiduAPI_ID = "";

		public static string baiduAPI_key = "";

		public static bool 截图排斥;

		public static bool v_topmost;

		public static Image v_screenimage;

		public static string v_date;
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using QuickJS;
using QuickJS.Native;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static QuickJS.Native.QuickJSNativeApi;
namespace TestApp
{
	public class Module
	{
		private static IntPtr funList=IntPtr.Zero;
		public unsafe static JSModuleDef InitModule(JSContext context, string name)
		{
			var r = 0;
			var m = JS_NewCModule(context, name, js_my_module_init);
			if (new IntPtr(m.ToPointer()) != IntPtr.Zero)
			{
			   var p = Marshal.GetFunctionPointerForDelegate(Add);

				js_my_module_funcs[0].SetEntry("add", p, 0);
				js_my_module_funcs[1].SetEntry("add2", p, 0);
				funList = Marshal.AllocHGlobal(Marshal.SizeOf(js_my_module_funcs[0]) * js_my_module_funcs.Length);

				//new Span<JSCFunctionListEntry>(js_my_module_funcs).CopyTo(new Span<JSCFunctionListEntry>((JSCFunctionListEntry*)funList, 2));
				long LongPtr = funList.ToInt64();
				for (int I = 0; I < js_my_module_funcs.Length; I++)
				{
					IntPtr RectPtr = new IntPtr(LongPtr);
					Marshal.StructureToPtr(js_my_module_funcs[I], RectPtr, false);
					LongPtr += Marshal.SizeOf(typeof(JSCFunctionListEntry));
				}


			  	r=JS_AddModuleExportList(context, m,(JSCFunctionListEntry*)funList, js_my_module_funcs.Length);

				//var p = Marshal.GetFunctionPointerForDelegate(Add);
				r=JS_AddModuleExport(context, m, "name");
				
			}
			return m;
		}
		//[UnmanagedCallersOnly]
		private unsafe static JSValue Add(JSContext ctx, JSValue thisArg, int argc, JSValue* argv)
		{
			if (JS_ToInt32(ctx, out int a, argv[0]) > 0)
			{
				if (JS_ToInt32(ctx, out int b, argv[1]) > 0)

					return JS_NewInt32(ctx, a + b);
			}
			return JS_NewInt32(ctx, 0);
		}

		//[UnmanagedCallersOnly]
		static unsafe int js_my_module_init(JSContext ctx, JSModuleDef m)
		{
			var r=	JS_SetModuleExport(ctx, m, "name", JSValue.Create(ctx,"TestName"));
			if (r==0)
			{
				r=	JS_AddModuleExportList(ctx, m, (JSCFunctionListEntry*)funList, js_my_module_funcs.Length);
			
			}

			return r;
		}

		 
		static JSCFunctionListEntry[] js_my_module_funcs = new JSCFunctionListEntry[2];

	}
}

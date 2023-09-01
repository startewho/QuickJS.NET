using System;
using System.Runtime.InteropServices;
using System.Text;
using QuickJS;
using QuickJS.Native;
using static QuickJS.Native.QuickJSNativeApi;

namespace TestApp
{
	class Program
	{
		unsafe static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			using (var rt = new QuickJSRuntime())
			{
				rt.StdInitHandlers();
				rt.SetDefaultModuleLoader();
				using (var context = rt.CreateContext())
				{
					context.StdAddHelpers();
					context.InitModuleStd("std");
					context.InitModuleOS("os");
				var m=	Module.InitModule(context.NativeInstance, "m");
					
					PrintHello(context);

					try
					{
						var path = "Js/test.js";
						int f = (int)JSEvalFlags.Module;
						context.EvalFile(path, Encoding.UTF8, JSEvalFlags.Module);
						
					}
					catch (QuickJSException ex)
					{
						Console.WriteLine(ex);
					}

					Console.WriteLine();
					//(context.EvalFile(@"G:\BUILD\QuickJS\repl.js", Encoding.ASCII, JSEvalFlags.Module | JSEvalFlags.Strip) as IDisposable)?.Dispose();
					rt.RunStdLoop(context);
				}
				rt.StdFreeHandlers();
			}

			Console.WriteLine();
			Console.WriteLine("press key");
			Console.ReadKey();
		}

		private unsafe static void PrintHello(QuickJSContext context)
		{
			QuickJSValue globalObj = context.GetGlobal();
			QuickJSValue obj = QuickJSValue.Create(context);
			globalObj.DefineFunction("hello", Hello, 1, 0, new[] { JSValue.Create(1), obj.NativeInstance }, JSPropertyFlags.CWE);
			GC.KeepAlive(obj);

			globalObj.DefineFunction("hello2", Hello2, 1, JSPropertyFlags.CWE);

			globalObj.DefineFunction("hello3", Hello3, 1, 0, new JSValue[0], JSPropertyFlags.CWE);

			try
			{
				context.Eval("hello(); hello2('World'); hello3(3);", "script.js", JSEvalFlags.Global);
			}
			catch (QuickJSException ex)
			{
				Console.WriteLine(ex);
			}
		}

		private unsafe static JSValue Hello(JSContext ctx, JSValue thisArg, int argc, JSValue[] argv, int magic, JSValue* data)
		{
			string name = argc > 0 ? argv[0].ToString(ctx) : "anonymous";
			Console.WriteLine($"Hello, {name}!");
			return JSValue.Undefined;
		}

		private unsafe static JSValue Hello2(JSContext ctx, JSValue thisArg, int argc, JSValue[] argv)
		{
			return Hello(ctx, thisArg, argc, argv, 0, null);
		}

		private unsafe static JSValue Hello3(JSContext ctx, JSValue thisArg, JSValue[] argv, int magic, JSValue[] data)
		{
			fixed (JSValue* fnData = data)
			{
				return Hello(ctx, thisArg, argv.Length, argv, magic, fnData);
			}
		}

	}
}

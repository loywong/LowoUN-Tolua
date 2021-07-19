﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class UtilWrap
{
	public static void Register(LuaState L)
	{
		L.BeginStaticLibs("Util");
		L.RegFunction("Instantiate2", Instantiate2);
		L.RegFunction("md5file", md5file);
		L.EndStaticLibs();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Instantiate2(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 1, typeof(UnityEngine.GameObject));
			UnityEngine.Transform arg1 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			UnityEngine.Transform o = Util.Instantiate2(arg0, arg1);
			ToLua.Push(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int md5file(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			string arg0 = ToLua.CheckString(L, 1);
			long arg1;
			string o = Util.md5file(arg0, out arg1);
			LuaDLL.lua_pushstring(L, o);
			LuaDLL.tolua_pushint64(L, arg1);
			return 2;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}


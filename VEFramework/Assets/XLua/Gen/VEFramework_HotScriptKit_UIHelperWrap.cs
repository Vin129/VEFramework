#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class VEFrameworkHotScriptKitUIHelperWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(VEFramework.HotScriptKit.UIHelper);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 36, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "FindChildGameObj", _m_FindChildGameObj_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetParent", _m_SetParent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetTransform", _m_SetTransform_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetLocalPosition", _m_SetLocalPosition_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetLabelText", _m_SetLabelText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetButtonInteractable", _m_SetButtonInteractable_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetButtonAble", _m_SetButtonAble_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetButtonClickEvent", _m_SetButtonClickEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveButtonClickEvent", _m_RemoveButtonClickEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetButtonClicked", _m_SetButtonClicked_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddDropdownEvent", _m_AddDropdownEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "AddDropdownOption", _m_AddDropdownOption_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveDropdownOption", _m_RemoveDropdownOption_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UpdateDropdownText", _m_UpdateDropdownText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetInputEvent", _m_SetInputEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetToggleEvent", _m_SetToggleEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetToggleIsOn", _m_GetToggleIsOn_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetToggleIsOn", _m_SetToggleIsOn_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetToggleEnabled", _m_SetToggleEnabled_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetToggleInteractable", _m_SetToggleInteractable_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetToggleGroup", _m_SetToggleGroup_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterDropdownClickEvent", _m_RegisterDropdownClickEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterClickEvent", _m_RegisterClickEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterPressedDownEvent", _m_RegisterPressedDownEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterPressedUpEvent", _m_RegisterPressedUpEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterBeginDragEvent", _m_RegisterBeginDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterDragEvent", _m_RegisterDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterEndDragEvent", _m_RegisterEndDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterNotDragEvent", _m_RegisterNotDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RemoveAllDragEvent", _m_RemoveAllDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RevertAllDragEvent", _m_RevertAllDragEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterDropEvent", _m_RegisterDropEvent_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "RegisterDropCallback", _m_RegisterDropCallback_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UnRegisterDropCallback", _m_UnRegisterDropCallback_xlua_st_);
            
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "no_breaking_space", VEFramework.HotScriptKit.UIHelper.no_breaking_space);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "VEFramework.HotScriptKit.UIHelper does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindChildGameObj_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    string _path = LuaAPI.lua_tostring(L, 2);
                    
                        var gen_ret = VEFramework.HotScriptKit.UIHelper.FindChildGameObj( _obj, _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetParent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    UnityEngine.GameObject _parent = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.SetParent( _obj, _parent );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetTransform_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    UnityEngine.RectTransform _transform = (UnityEngine.RectTransform)translator.GetObject(L, 2, typeof(UnityEngine.RectTransform));
                    
                    VEFramework.HotScriptKit.UIHelper.SetTransform( _kObj, _transform );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLocalPosition_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    UnityEngine.Vector3 _pos;translator.Get(L, 2, out _pos);
                    
                    VEFramework.HotScriptKit.UIHelper.SetLocalPosition( _kObj, _pos );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLabelText_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    string _strText = LuaAPI.lua_tostring(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetLabelText( _kObj, _strText );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetButtonInteractable_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    bool _interactable = LuaAPI.lua_toboolean(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetButtonInteractable( _kObj, _interactable );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetButtonAble_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    bool _able = LuaAPI.lua_toboolean(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetButtonAble( _kObj, _able );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetButtonClickEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.SetButtonClickEvent( _kObj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveButtonClickEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.RemoveButtonClickEvent( _kObj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetButtonClicked_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.SetButtonClicked( _kObj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddDropdownEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.AddDropdownEvent( _kObj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddDropdownOption_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    string _text = LuaAPI.lua_tostring(L, 2);
                    string _textDst = LuaAPI.lua_tostring(L, 3);
                    
                    VEFramework.HotScriptKit.UIHelper.AddDropdownOption( _kObj, _text, _textDst );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveDropdownOption_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    int _index = LuaAPI.xlua_tointeger(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.RemoveDropdownOption( _kObj, _index );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UpdateDropdownText_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    int _index = LuaAPI.xlua_tointeger(L, 2);
                    string _text = LuaAPI.lua_tostring(L, 3);
                    
                    VEFramework.HotScriptKit.UIHelper.UpdateDropdownText( _kObj, _index, _text );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetInputEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.SetInputEvent( _kObj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetToggleEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.SetToggleEvent( _kObj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetToggleIsOn_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                        var gen_ret = VEFramework.HotScriptKit.UIHelper.GetToggleIsOn( _kObj );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetToggleIsOn_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    bool _isOn = LuaAPI.lua_toboolean(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetToggleIsOn( _kObj, _isOn );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetToggleEnabled_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    bool _enabled = LuaAPI.lua_toboolean(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetToggleEnabled( _kObj, _enabled );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetToggleInteractable_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    bool _interactable = LuaAPI.lua_toboolean(L, 2);
                    
                    VEFramework.HotScriptKit.UIHelper.SetToggleInteractable( _kObj, _interactable );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetToggleGroup_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _kObj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    UnityEngine.GameObject _gObj = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.SetToggleGroup( _kObj, _gObj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterDropdownClickEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterDropdownClickEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterClickEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterClickEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterPressedDownEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterPressedDownEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterPressedUpEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterPressedUpEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterBeginDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterBeginDragEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterDragEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterEndDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterEndDragEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterNotDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterNotDragEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveAllDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.RemoveAllDragEvent( _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RevertAllDragEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.RevertAllDragEvent( _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterDropEvent_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _kLuaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterDropEvent( _obj, _kLuaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RegisterDropCallback_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    object _luaFunc = translator.GetObject(L, 2, typeof(object));
                    
                    VEFramework.HotScriptKit.UIHelper.RegisterDropCallback( _obj, _luaFunc );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnRegisterDropCallback_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.GameObject _obj = (UnityEngine.GameObject)translator.GetObject(L, 1, typeof(UnityEngine.GameObject));
                    
                    VEFramework.HotScriptKit.UIHelper.UnRegisterDropCallback( _obj );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}

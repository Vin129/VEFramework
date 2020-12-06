using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;
using UnityEngine;
using VEFramework;
using VEFramework.HotScriptKit;

public static class VXLuaConfig
{
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
        // typeof(DG.Tweening.DOTween),
        // typeof(DG.Tweening.Tween),
        // typeof(DG.Tweening.Sequence),
        // typeof(DG.Tweening.Tweener),
        // typeof(DG.Tweening.LoopType),
        // typeof(DG.Tweening.PathMode),
        // typeof(DG.Tweening.PathType),
        // typeof(DG.Tweening.RotateMode),
        typeof(Component),
        typeof(Transform),
        typeof(Light),
        typeof(Material),
        typeof(Rigidbody),
        typeof(Camera),
        typeof(AudioSource),

          
        typeof(Behaviour),
        typeof(MonoBehaviour),        
        typeof(GameObject),
        typeof(TrackedReference),
        typeof(Application),
        typeof(Physics),
        typeof(Collider),
        typeof(Time),        
        typeof(Texture),
        typeof(Texture2D),
        typeof(Shader),        
        typeof(Renderer),
        typeof(Screen),        
        typeof(CameraClearFlags),
        typeof(AudioClip),        
        typeof(AssetBundle),
        typeof(AsyncOperation),       
        typeof(LightType),
        typeof(SleepTimeout),

        typeof(LuaHelper),
        typeof(UIHelper),

        
#if UNITY_5_3_OR_NEWER && !UNITY_5_6_OR_NEWER
        typeof(UnityEngine.Experimental.Director.DirectorPlayer),
#endif
        typeof(Animator),
        typeof(Input),
        typeof(KeyCode),
        typeof(SkinnedMeshRenderer),
        typeof(Space),      
       

        typeof(MeshRenderer),
#if !UNITY_5_4_OR_NEWER
        typeof(ParticleEmitter),
        typeof(ParticleRenderer),
        typeof(ParticleAnimator), 
#endif

        typeof(BoxCollider),
        typeof(MeshCollider),
        typeof(SphereCollider),        
        typeof(CharacterController),
        typeof(CapsuleCollider),
        
        typeof(Animation),        
        typeof(AnimationClip),        
        typeof(AnimationState),
        typeof(AnimationBlendMode),
        typeof(QueueMode),  
        typeof(PlayMode),
        typeof(WrapMode),

        typeof(QualitySettings),
        typeof(RenderSettings),                                                             
        typeof(RenderTexture),
        typeof(Resources),     
    };

    // 自动把LuaCallCSharp涉及到的delegate加到CSharpCallLua列表，后续可以直接用lua函数做callback
    [CSharpCallLua]
    public static List<Type> CSharpCallLua
    {
       get
       {
           var lua_call_csharp = LuaCallCSharp;
           var delegate_types = new List<Type>();
           var flag = BindingFlags.Public | BindingFlags.Instance
               | BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly;
           foreach (var field in (from type in lua_call_csharp select type).SelectMany(type => type.GetFields(flag)))
           {
               if (typeof(Delegate).IsAssignableFrom(field.FieldType))
               {
                   delegate_types.Add(field.FieldType);
               }
           }

           foreach (var method in (from type in lua_call_csharp select type).SelectMany(type => type.GetMethods(flag)))
           {
               if (typeof(Delegate).IsAssignableFrom(method.ReturnType))
               {
                   delegate_types.Add(method.ReturnType);
               }
               foreach (var param in method.GetParameters())
               {
                   var paramType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                   if (typeof(Delegate).IsAssignableFrom(paramType))
                   {
                       delegate_types.Add(paramType);
                   }
               }
           }
           return delegate_types.Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct().ToList();
       }
    }

//    [Hotfix]
//     static IEnumerable<Type> HotfixInject
//     {
//        get
//        {
//            return (from type in Assembly.Load("Assembly-CSharp").GetTypes()
//                    where type.Namespace == null || !type.Namespace.StartsWith("XLua")
//                    select type);
//        }
//     }
    //--------------begin 热补丁自动化配置-------------------------
    static bool hasGenericParameter(Type type)
    {
       if (type.IsGenericTypeDefinition) return true;
       if (type.IsGenericParameter) return true;
       if (type.IsByRef || type.IsArray)
       {
           return hasGenericParameter(type.GetElementType());
       }
       if (type.IsGenericType)
       {
           foreach (var typeArg in type.GetGenericArguments())
           {
               if (hasGenericParameter(typeArg))
               {
                   return true;
               }
           }
       }
       return false;
    }

    static bool typeHasEditorRef(Type type)
    {
       if (type.Namespace != null && (type.Namespace == "UnityEditor" || type.Namespace.StartsWith("UnityEditor.")))
       {
           return true;
       }
       if (type.IsNested)
       {
           return typeHasEditorRef(type.DeclaringType);
       }
       if (type.IsByRef || type.IsArray)
       {
           return typeHasEditorRef(type.GetElementType());
       }
       if (type.IsGenericType)
       {
           foreach (var typeArg in type.GetGenericArguments())
           {
               if (typeHasEditorRef(typeArg))
               {
                   return true;
               }
           }
       }
       return false;
    }

    static bool delegateHasEditorRef(Type delegateType)
    {
       if (typeHasEditorRef(delegateType)) return true;
       var method = delegateType.GetMethod("Invoke");
       if (method == null)
       {
           return false;
       }
       if (typeHasEditorRef(method.ReturnType)) return true;
       return method.GetParameters().Any(pinfo => typeHasEditorRef(pinfo.ParameterType));
    }

    // 配置某Assembly下所有涉及到的delegate到CSharpCallLua下，Hotfix下拿不准那些delegate需要适配到lua function可以这么配置
    // [CSharpCallLua]
    // static IEnumerable<Type> AllDelegate
    // {
    //    get
    //    {
    //        BindingFlags flag = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
    //        List<Type> allTypes = new List<Type>();
    //        var allAssemblys = new Assembly[]
    //        {
    //            Assembly.Load("Assembly-CSharp")
    //        };
    //        foreach (var t in (from assembly in allAssemblys from type in assembly.GetTypes() select type))
    //        {
    //            var p = t;
    //            while (p != null)
    //            {
    //                allTypes.Add(p);
    //                p = p.BaseType;
    //            }
    //        }
    //        allTypes = allTypes.Distinct().ToList();
    //        var allMethods = from type in allTypes
    //                         from method in type.GetMethods(flag)
    //                         select method;
    //        var returnTypes = from method in allMethods
    //                          select method.ReturnType;
    //        var paramTypes = allMethods.SelectMany(m => m.GetParameters()).Select(pinfo => pinfo.ParameterType.IsByRef ? pinfo.ParameterType.GetElementType() : pinfo.ParameterType);
    //        var fieldTypes = from type in allTypes
    //                         from field in type.GetFields(flag)
    //                         select field.FieldType;
    //        return (returnTypes.Concat(paramTypes).Concat(fieldTypes)).Where(t => t.BaseType == typeof(MulticastDelegate) && !hasGenericParameter(t) && !delegateHasEditorRef(t)).Distinct();
    //    }
    // }
    //--------------end 热补丁自动化配置-------------------------

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            };

#if UNITY_2018_1_OR_NEWER
    [BlackList]
    public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
    {
        if (memberInfo.DeclaringType.IsGenericType && memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            if (memberInfo.MemberType == MemberTypes.Constructor)
            {
                ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                var parameterInfos = constructorInfo.GetParameters();
                if (parameterInfos.Length > 0)
                {
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                    {
                        return true;
                    }
                }
            }
            else if (memberInfo.MemberType == MemberTypes.Method)
            {
                var methodInfo = memberInfo as MethodInfo;
                if (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2)
                {
                    return true;
                }
            }
        }
        return false;
    };
#endif
}

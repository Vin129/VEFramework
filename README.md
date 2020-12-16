# VEFramework
**VEFramwork = Vin129(Very) + Easy + Framwork  : )**



# V0.0.1

**Core**

- **DisignMode  : 设计模式** 

- **Manager : ManagerOfManagers   对外由VEManager来统领全部**


  - **EasyLog： BaseLog**


  - **VUI: UI层框架**
    - **VUIKit: UI框架  【V0.1 最小可用UI模块】**

  - **VAsset  : 资产管理    细化承保单位（Assurer） 跟踪化管理**  
- **VAssetKit : VAsset核心模块  【V0.1  VAsset 统筹管理】**
  
- **ResourceKit : Resources资产模块  【V0.1  ResManager 】**
  
- **AssestBundleKit : AB资产模块  【V0.1  ABManager + ABBuilder】**
  
- **NetAssetKit : 网络资产模块  【V0.1  NetAssetManager 】**
  
- **Rule：规则模块**









# V0.0.2

**Core**

- **~~DisignMode  : 设计模式~~** 

- **~~Manager : ManagerOfManagers   对外由VEManager来统领全部~~**


  - **EasyLog：三层Log输出，轻松查询日志（Unity、GUI、File）**
  - **EasyNet：兼容HTTP、Socket、WebSocket**
    - **EasyHTTP**
    - **EasySocket**
    - **EasyWebSocket**


  - **VUI: UI层框架**

    - **VUIKit: UI框架 **
      - **提供三种写UI的支持：纯C#、C#+lua、纯lua**
      - **UI界面管理+层级控制+自定义层级支持 **
      - **VLoader：资源无忧释放**
      - **VisualizeScript 脚本可视化**
    - **VUGUI: UGUI改良**
  -  **VAsset  : 资产管理    细化承保单位（Assurer） 跟踪化管理**  

    - **~~VAssetKit : VAsset核心模块  【V0.1  VAsset 统筹管理】~~**
    - **~~ResourceKit : Resources资产模块  【V0.1  ResManager 】~~**
    - **~~AssestBundleKit : AB资产模块  【V0.1  ABManager + ABBuilder】~~**
    - **~~NetAssetKit : 网络资产模块  【V0.1  NetAssetManager 】~~**
    - **~~Rule：规则模块~~**
    - **AssurerViewer：运行时资产可视化**
    - **AssetFinder：检查资产是否被使用** 
- **HotScriptKit**

    - **LuaKit**
      - **~~ToLua v0.1~~**
        - **~~XLua v0.1~~**
        - **~~QuickLuaViewer v0.1~~** 
        - **LuaPacker**
- **VEPackageManager： VE相关Packages 管理整合**

    - **~~LocalPackages : 本地Packages 管理 v0.1~~**
    - **DownloadPackages ：下载**
    - **Packages收录**
      - **~~HotScriptKit_Tolua~~**
      - **~~HotScriptKit_XLua~~ **










**— GamePipeline : Easy化使用流（非开发者）**

​	**— 资产可视化  VAssetEditor：资产可视化工具 **



## TimeLine

- **AssetBundleKit 初步完成（基于相对路径的资源加载模式**
- **AssetBundleKit 完成LazyRelease（I_DONT_CARE）模式**
- **AssetBundleKit ABBuild模块初步构成**
- **AssetBundleKit V0.1版本构建完成（ABManager+ABBuilder）**
- **ResourceKit 完成 v0.1**
- **VAsset 设立对外总入口**
- **NetAssetKit 完成 v0.1**
- **EasyLog模块 v0.1**
- **VUI v0.1**
- **HotScript v0.1**
- **VEPackageManager v0.1**



***

#  工作流 

 **~~1.十月计划：VUI + LuaKit~~ **

 **~~2.十一月计划：VUI  + 可视化 UIScript~~ **

**3.十二月计划：XLua + URP**



## TODO

**~~AssetBundleKit 释放Assurer机制~~**

**~~AssetBundleKit 制作对应的ABBuild工具~~**

**~~AssetBundleKit Editor工具模块~~**

**~~ABAssurer 释放存在风险，设计一套安全的方式~~**

**~~VAsset 完善Resources部分与AssetLoad部分~~**

**~~VAssetKit 统筹管理：Asset、Resource、AssetBundle、WWW~~**

**~~VAsset 完成 v0.1版本~~**

**~~UIKit  (UICanvas) v0.1~~**

**~~BaseUI &  LuaKit~~**

**~~VAsset升级，Assurer管理模块改造，更适合各种环境下合理使用。~~**

**~~VAsset:梳理 释放模式~~**

**~~XLua 支持~~**

**~~XLua的加载支持~~**

**~~QuickLuaViewer 升级 ，公共管理~~**

**Lua脚本打包便捷工具**

**Shader + URP模块**

**VAssetEditor模块：Assurer 可视化窗口 升级**

**ABManager：更精确的加载**

**VAssetEditor模块：AssetFinder**

**EasyLog模块**

**尝试IOC设计**

**EasyNet模块整理**

**可视化 UIScript**

**VAciton**



**N.VSDK**

**~~【Obsolete】15.VAsset : Addressable Asset System~~**


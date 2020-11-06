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
      - **资源无忧释放**
      - **VisualizeScript 脚本可视化**
    - **VUGUI: UGUI改良**

  -  **VAsset  : 资产管理    细化承保单位（Assurer） 跟踪化管理**  

    - **~~VAssetKit : VAsset核心模块  【V0.1  VAsset 统筹管理】~~**
    - **~~ResourceKit : Resources资产模块  【V0.1  ResManager 】~~**
    - **~~AssestBundleKit : AB资产模块  【V0.1  ABManager + ABBuilder】~~**
    - **~~NetAssetKit : 网络资产模块  【V0.1  NetAssetManager 】~~**
    - **~~Rule：规则模块~~**
    - **AssetFinder：资产使用可视化** 
    
- **HotScriptKit**

- **LuaKit**
  
  - **~~ToLua~~**
  
  
  
  
  
  



 

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



***

#  工作流 

 **1.十月计划：VUI + LuaKit + VisualizeScript**

 **2.十一月计划：VUI  + 可视化 UIScript**





## TODO

**~~1.AssetBundleKit 释放Assurer机制~~**

**~~2.AssetBundleKit 制作对应的ABBuild工具~~**

**~~3.AssetBundleKit Editor工具模块~~**

**~~4.ABAssurer 释放存在风险，设计一套安全的方式~~**

**~~5.VAsset 完善Resources部分与AssetLoad部分~~**

**~~6.VAssetKit 统筹管理：Asset、Resource、AssetBundle、WWW~~**

**~~7.VAsset 完成 v0.1版本~~**

**8.EasyLog模块**

**9.尝试IOC设计**

**10.EasyNet模块整理**

**11. VAssetEditor模块**

**12.AssetFinder**

**~~13.UIKit (UICanvas) v0.1~~**

**~~14.BaseUI &  LuaKit~~**

**15.可视化 UIScript**

**16.参考AddressableAsset 引用计数升级VAsset**

**17.VAsset升级，Assurer管理模块改造，更适合各种环境下合理使用。**



**N.VSDK**

**~~【Obsolete】15.VAsset : Addressable Asset System~~**


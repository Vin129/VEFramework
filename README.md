# VEFramework
**VEFramwork = Vin129(Very) + Easy + Framwork  : )**



# V0.0.1

**— Core**

​	**— DisignMode  : 设计模式**

​	**— Manager : ManagerOfManagers   对外由VEManager来统领全部**

​	**— VAsset  : 资产管理    细化承保单位（Assurer） 跟踪化管理**

​		**— VAssetKit : VAsset核心模块  【V0.1  VAsset 统筹管理】**

​		**— ResourceKit : Resources资产模块  【V0.1  ResManager 】**

​		**— AssestBundleKit : AB资产模块  【V0.1  ABManager + ABBuilder】**

​		**— NetAssetKit : 网络资产模块  【V0.1  NetAssetManager 】**

​		**— Rule：规则模块**



**— GamePipeline : Easy化使用流（非开发者）**





## TimeLine

### 8月

**7-26 :   AssetBundleKit 初步完成（基于相对路径的资源加载模式）**

**7-29： AssetBundleKit 完成LazyRelease（I_DONT_CARE）模式**

**7-31:    AssetBundleKit ABBuild模块初步构成**

**8-2:    AssetBundleKit V0.1版本构建完成（ABManager+ABBuilder） 注：ABAssurer若对外界开放则释放层面存在风险**

**8-4:  ResourceKit 完成 v0.1**

**8-5:  VAsset 设立对外总入口**

**8-8:  NetAssetKit 完成 v0.1**

## TODO

**~~1.AssetBundleKit 释放Assurer机制~~**

**~~2.AssetBundleKit 制作对应的ABBuild工具~~**

**~~3.AssetBundleKit Editor工具模块~~**

**~~4.ABAssurer 释放存在风险，设计一套安全的方式~~**

**~~5.VAsset 完善Resources部分与AssetLoad部分~~**

**~~6.VAssetKit 统筹管理：Asset、Resource、AssetBundle、WWW~~**

**7.VAsset 完成 v0.1版本**


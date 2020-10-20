using System;
//暂弃
namespace LuaKit {
	public class PlugCollector {
		private LuaPlug plug;
		public PlugCollector(){}
		public IPlug GetPlug(){
			return plug;
		}
		public void InitPlug(){
			plug = LuaPlug.GetInstance();
			plug.Init();
		}
	}
}

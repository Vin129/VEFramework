using System.Collections;
namespace LuaKit {
	public interface IPlug  {
		int PlugId {get;}

		void Init();
		void Destroy();
	}
}

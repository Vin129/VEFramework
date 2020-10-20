namespace LuaKit {
	using VEFramework;
	public class BaseOutlet : Singleton<BaseOutlet>
	{
		private PlugCollector mPlugCtor;
		protected BaseOutlet()
		{

		}
		public override void InitFinished(){
			mPlugCtor = new PlugCollector();
		}

		public void Init(){
			mPlugCtor.InitPlug();
		}
	}
}

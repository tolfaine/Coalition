using UnityEngine;

namespace interfaces
{
	public class SampleContext : HelloWorldContext
	{
		public SampleContext(MonoBehaviour contextView, bool autostart)
			: base(contextView, autostart)
		{
		}

		public SampleContext(MonoBehaviour contextView, bool autostart, InterfaceMap map)
			: base(contextView, autostart, map)
		{
		}

		protected override void mapBindings()
		{
			base.mapBindings();

			commandBinder.Bind<StartSignal>()  //;
			.To<LogClientInfoCommand>();
			//.To<LoadMainLevelCommand>();
		}
	}
}
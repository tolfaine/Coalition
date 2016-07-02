using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.impl;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	//Base Class for all our Contexts
	//We do two jobs here:
	//1. Provide the important bindings for a Signals-based app (see http://strangeioc.wordpress.com/2013/09/18/signals-vs-events-a-strange-smackdown-part-2-of-2/)
	//2. Scan for Implicit Bindings (see http://strangeioc.wordpress.com/2013/12/16/implicitly-delicious/)

	public class SignalContext : MVCSContext
	{
		public List<string> NamespacesToScan = new List<string> { "interfaces", "implementations" };

		public SignalContext()
		{
		}

		public SignalContext(MonoBehaviour contextView)
			: base(contextView)
		{
		}

		public SignalContext(MonoBehaviour contextView, bool autostart)
			: base(contextView, autostart)
		{
		}

		protected override void addCoreComponents()
		{
			base.addCoreComponents();
			injectionBinder.Unbind<ICommandBinder>();
			injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
		}

		public override void Launch()
		{
			base.Launch();
			StartSignal startSignal = (StartSignal)injectionBinder.GetInstance<StartSignal>();
			startSignal.Dispatch();
		}

		protected override void mapBindings()
		{
			base.mapBindings();
			implicitBinder.ScanForAnnotatedClasses(NamespacesToScan.ToArray());
		}
	}

	public class StartSignal : Signal
	{
	}
}
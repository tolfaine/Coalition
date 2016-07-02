using System.Collections;

namespace interfaces
{
	public class SampleContextView : HelloWorldContextView
	{
		protected new void Start()
		{
			SetFrameRate();
			StartCoroutine(MyLoadContext());
		}

		private IEnumerator MyLoadContext()
		{
			yield return StartCoroutine(LoadInterfaceMap(false));
			context = new SampleContext(this, true, _interfaceMap);
			context.Start();
		}
	}
}
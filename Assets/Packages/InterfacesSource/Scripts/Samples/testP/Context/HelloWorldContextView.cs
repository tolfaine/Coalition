using strange.extensions.context.impl;
using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class HelloWorldContextView : ContextView
	{
		// Use this for initialization
		[SerializeField] //todo: write serialization code?
		protected InterfaceMap defaultMappings;

		protected InterfaceMap _interfaceMap;

		protected void SetFrameRate()
		{
			if(Application.isEditor || (!Application.isMobilePlatform))
				Application.targetFrameRate = 75;
			else
				Application.targetFrameRate = 60;
		}

		protected void Start()
		{
			SetFrameRate();
			StartCoroutine(LoadHelloWorldLoadContext());
		}

		protected InterfaceMap GetDefaultMap()
		{
			var mapJson = Resources.Load<TextAsset>("DefaultMappings");
			return mapJson.text.JsonDeserialize<InterfaceMap>();
		}

		protected IEnumerator LoadHelloWorldLoadContext()
		{
			yield return StartCoroutine(LoadInterfaceMap(false));
			context = new HelloWorldContext(this, true, _interfaceMap);
			context.Start();
		}

		protected IEnumerator LoadInterfaceMap(bool loadFromNet)
		{
			_interfaceMap = GetDefaultMap();
			if(_interfaceMap == null)
				Debug.LogError("Default interface map is not set up properly!");

			if(loadFromNet)
			{
				var requestor = new DownloadInterfaceMapFromMyHost();
				yield return StartCoroutine(requestor.getCollabEditDocument());
				if(requestor.output.map == null)
					requestor.output.map = defaultMappings;
				if(requestor.output != null && requestor.output.map != null)
					_interfaceMap = requestor.output.map;
			}

			//yield return null;
		}
	}
}
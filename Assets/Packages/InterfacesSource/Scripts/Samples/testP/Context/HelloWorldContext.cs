using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace interfaces
{
	public enum NamedInjectionsCore
	{
		PRODUCT_NAME,
		BUILD_TYPE,
		BUILD_NUMBER,
		PLATFORM,
	}

	public class HelloWorldContext : SignalContext
	{
		protected InterfaceMap _map;

		protected List<Type> defaultAnalyticsMap = new List<Type>
		{
			typeof (NullAnalytics),
			typeof (NullAnalytics),
			typeof (NullAnalytics)
		};

		protected List<Type> defaultLoggers = new List<Type>
		{
			typeof (RemoteLogger),
			typeof (UnityLogger),
			typeof (NullDebugLog)
		};

		public HelloWorldContext(MonoBehaviour contextView, bool autostart)
			: base(contextView, autostart)
		{
		}

		public HelloWorldContext(MonoBehaviour contextView, bool autostart, InterfaceMap map)
			: base(contextView, autostart)
		{
			_map = map;
		}

		protected override void mapBindings()
		{
			base.mapBindings();
			var helper = new ReflectionHelpers();
			//map singletons
			foreach(var kvp in _map.interfaceMap)
			{
				//injectionBinder.Bind(helper.GetTypeInAssemblies(kvp.Key))
				//	.ToValue(Activator.CreateInstance(helper.GetTypeInAssemblies(kvp.Value))).ToSingleton();
				injectionBinder.Bind(helper.GetTypeInAssemblies(kvp.Key)).To(helper.GetTypeInAssemblies(kvp.Value)).ToSingleton();
			}

			foreach(var namedStringType in _map.namedStringTypes)
			{
				injectionBinder.Bind(namedStringType.Key).To(namedStringType.Value);
			}
			GetPlatform();
			SetupAnalytics(helper);

			var buildConfig = ResourceLoader.Load<BuildConfigSO>();
			injectionBinder.Bind<BuildConfigSO>().ToValue(buildConfig);

			IVR vrCamera = mapCamera(helper);

			injectionBinder.Bind<IRoutineRunner>().ToValue(SingletonManager.instance.AddSingleton<RoutineRunner>());
			//{"interfaceMap":{"IController":"UnityDefaultInputAxisController","IVR":"OculusDesktopVR"},"interfaceToMultipleMaps":{"IAnalytics":["CountlyAnalytics"]},"namedStringTypes":{},"namedIntTypes":{}}
			const string CameraSettingsJsonName = "CameraSettingsJson";
			var cameraSettingsJson = _map.namedStringTypes.ContainsKey(CameraSettingsJsonName)
				? _map.namedStringTypes[CameraSettingsJsonName]
				: null;
			LoadCamera(vrCamera, vrCamera.GetCameraSettingsSO(), cameraSettingsJson);
			
			var lifecycle = SingletonManager.instance.AddSingleton<MonobehaviourLifecycleMessages>();
			injectionBinder.Bind<IMonobehaviourLifecycleMessages>().ToValue(lifecycle);

			SetupLoggers(helper);

			//NOTE: not setting StartSignal mapping, leaving that up to subloggers!
		}

		protected void GetPlatform()
		{
			string platformString = Application.platform.ToString();
			platformString = platformString.Replace("WindowsEditor", "StandaloneWindows");
			if(_map.namedStringTypes != null && _map.namedStringTypes.ContainsKey(NamedInjectionsCore.PLATFORM.ToString()))
			{
				platformString = _map.namedStringTypes[NamedInjectionsCore.PLATFORM.ToString()];
			}

			injectionBinder.Bind<string>().ToName(NamedInjectionsCore.PLATFORM).ToValue(platformString);
			//Debug.Log("platform:" + NamedInjectionsCore.PLATFORM.ToString() + " and used key:" + platformString);
		}

		protected void SetupWebService()
		{
			injectionBinder.Bind<IWebService>().To<UnityWWWWebSerivce>();
		}

		protected void SetupAnalytics(ReflectionHelpers helper)
		{
			string analyticsTypeName = typeof(IAnalytics).Name;
			List<Type> analyticsSystems = new List<Type>();

			if(_map.interfaceToMultipleMaps.ContainsKey(analyticsTypeName))
			{
				var multiAnalytics = _map.interfaceToMultipleMaps[analyticsTypeName];

				foreach(var analyticsSystemName in multiAnalytics)
				{
					try
					{
						var system = helper.GetTypeInAssemblies(analyticsSystemName);
						if(system == null)
						{
							Debug.LogError("Didd not find desired analytics system:" + analyticsSystemName);
							continue;
						}
						analyticsSystems.Add(system);
					}
					catch(Exception e)
					{
						Debug.LogError("error setting up analytics system:" + analyticsSystemName + " error:" + e);
					}
				}
			}
			string initStringKey = InterfaceNamedInjections.ANALYTICS_INITIALIZATION.ToString();
			List<string> listOfInitParams = new List<string>();
			try
			{
				if(_map.namedStringTypes.ContainsKey(initStringKey))
				{
					string initString = _map.namedStringTypes[initStringKey];
					if(initString != null)
					{
						listOfInitParams = initString.JsonDeserialize<List<string>>();
					}
				}
			}
			catch(Exception e)
			{
				Debug.LogError("Error while decodnig analytics init:" + e); //fall back to null analytics here?
			}

			injectionBinder.Bind<List<string>>()
							.ToValue(listOfInitParams)
							.ToName(InterfaceNamedInjections.ANALYTICS_INITIALIZATION);

			List<IAnalytics> analyticsSystemInstances = new List<IAnalytics>();

			foreach(var analyticsSystem in analyticsSystems)
			{
				//Debug.Log("Initializing analytics sytem:" + analyticsSystem.Name);
				analyticsSystemInstances.Add(Activator.CreateInstance(analyticsSystem) as IAnalytics);
			}

			injectionBinder.Bind<List<IAnalytics>>().ToValue(analyticsSystemInstances);
			injectionBinder.Bind<IAnalytics>().To<AnalyticsManager>().ToSingleton();
		}

		protected void SetupLoggers(ReflectionHelpers helper)
		{
			var mappedLoggers = false;
			if(_map.namedInterfaceBindings != null)
			{
				foreach(var namedInterfaceBinding in _map.namedInterfaceBindings)
				{
					if(namedInterfaceBinding.InterfaceToMap == typeof(ILogger).ToString())
					{
						mappedLoggers = true;
						injectionBinder.Bind<ILogger>()
							.To(helper.GetTypeInAssemblies(namedInterfaceBinding.TargetType))
							.ToName(namedInterfaceBinding.nameToMapTo);
						//injectionBinder.Bind<ILogger>().To<RemoteLogger>().ToName(namedInterfaceBinding.nameToMapTo);
					}
				}
			}
			if(!mappedLoggers)
			{
				injectionBinder.Bind<ILogger>().To<RemoteLogger>().ToName(LogManager.NamedLogger.PRIMARY);
				injectionBinder.Bind<ILogger>().To<UnityLogger>().ToName(LogManager.NamedLogger.SECONDARY);
				injectionBinder.Bind<ILogger>().To<NullDebugLog>().ToName(LogManager.NamedLogger.TERTIARY);
			}
			injectionBinder.Bind<ILogger>().To<LogManager>().ToSingleton().CrossContext();
			//injectionBinder.Bind<>()
		}

		protected IVR mapCamera(ReflectionHelpers helper)
		{
			IVR vrCamera = null;
			if(_map.interfaceMap.ContainsKey("IVR"))
			{
				try
				{
					vrCamera = Activator.CreateInstance(helper.GetTypeInAssemblies(_map.interfaceMap["IVR"])) as IVR;
				}
				catch(Exception e)
				{
					//TODO: find a way to get the real logger!
					Debug.LogError("ERROR_INITIALIXING_IVR_FROM_MAP:" + e);
				}
			}
			if(vrCamera == null)
			{
				if(injectionBinder.GetBinding<IVR>() != null)
				{
					vrCamera = injectionBinder.GetInstance<IVR>();
				}
			}

			if(vrCamera == null)
				vrCamera = new NullVR();
			var ivrbinding = injectionBinder.GetBinding<IVR>();
			if (ivrbinding == null)
			{
				injectionBinder.Bind<IVR>().ToValue(vrCamera).ToSingleton();
			}
			return vrCamera;
		}
		protected void LoadCamera(IVR vr, CameraSettingsSO cameraSettings, string cameraJsonSettings = null)
		{
			if(cameraJsonSettings == null)
			{
				cameraJsonSettings =
					@"{""clearFlags"":""Skybox"",""backgroundColor"":[0.13,0.15,0.2,0.01],""depth"":-1,""clearFlags"":""Skybox"",""textureResourcesName"":""Skybox/Skybox""}";
			}
			var cameraPrefab = Resources.Load<VRCameraFactory>("Camera");

			var cameraGOInstance = Object.Instantiate(cameraPrefab.gameObject) as GameObject;
			//make sure this doesn't end up in the scene perm
			//VRCameraFactory.SetHideflagsRecursive(cameraGOInstance, HideFlags.DontSave | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);

			var cameraFactory = cameraGOInstance.GetComponent<VRCameraFactory>();
			var cursorController = cameraGOInstance.GetComponent<CursorController>();
			var fadeCube = cameraGOInstance.GetComponentInChildren<FadeSphere>();

			cameraFactory.SetCameraSettingsSO(cameraSettings);

			injectionBinder.Bind<CursorController>().ToValue(cursorController);
			injectionBinder.Bind<FadeSphere>().ToValue(fadeCube);
			injectionBinder.Bind<VRCameraFactory>().ToValue(cameraFactory);
			injectionBinder.GetBinding<IVR>().ToSingleton(); //let's see if this fixes the IVR being imported as a factory...
			cameraFactory.enabled = true;
			var serializedCamera = cameraJsonSettings.JsonDeserialize<SerializedCameraSettings>();
			cameraFactory.SetCameraSettingsOnAllCameras(serializedCamera);
			if(serializedCamera.customCameraSettings != null)
				vr.SetCustomCameraSettings(serializedCamera.customCameraSettings);
			//cameraPrefab.enabled = true;
		}

		protected override void postBindings()
		{
			//if(Context.firstContext == this)
			//{
			//Debug.Log("PostBindings");
			base.postBindings();
		}
	}
}
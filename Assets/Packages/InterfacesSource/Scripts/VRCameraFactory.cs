using System;
using UnityEngine;

namespace interfaces
{
	public class VRCameraFactory : MonoBehaviour
	{
		private GameObject _currentCamera;

		//default oculus  {"clearFlags":"Skybox","backgroundColor":[0.192156866,0.3019608,0.4745098,0.0196078438],"fieldOfView":0,"depth":0}
		[SerializeField]
		private string _jsonCameraSettingsToUse;
		public HideFlags hideFlagsToUse = HideFlags.DontSave;
		[SerializeField]
		private CameraSettingsSO RuntimeCameraToUse; //set in each platform?
		//maybe put this in a serializableobject or something?  so each platform can set it up as appropriate?

		public GameObject GetCurrentCamera()
		{
			return _currentCamera;
		}

		public void SetCameraSettingsSO(CameraSettingsSO runtime)
		{
			RuntimeCameraToUse = runtime;
			createCamera();
		}

		[ContextMenu("Set Camera settings from string")]
		public void SetCameraSettingsFromDataSavedInInspector()
		{
			SetCameraSettingsOnAllCameras(_jsonCameraSettingsToUse.JsonDeserialize<SerializedCameraSettings>());
		}

		void createCamera()
		{
			destroyCamera();
			var vrCameraType = RuntimeCameraToUse;
			_currentCamera = vrCameraType.Create();
			Vector3 cameraOffsetPosition = Vector3.zero;
			_currentCamera.name = vrCameraType + " Editor Only(Do not edit)";
			_currentCamera.transform.parent = transform;
			_currentCamera.transform.localPosition = cameraOffsetPosition;
		}

		public void SetCameraSettingsOnAllCameras(SerializedCameraSettings settings)
		{
			var cameras = _currentCamera.GetComponentsInChildren<Camera>();
			foreach(var vrcamera in cameras)
			{
				if(settings.backgroundColor != null)
					vrcamera.backgroundColor = settings.getCameraBackgroundColor();
				vrcamera.clearFlags = settings.clearFlags;
				if(settings.farClipPlane > 0)
				{
					vrcamera.farClipPlane = settings.farClipPlane;
					//NOTE: almost every camera type seems to hard error whith this set to zero, so I don't dtho that for sure in code.  was the last but at ggj
				}

				if(settings.clearFlags == CameraClearFlags.Skybox && !string.IsNullOrEmpty(settings.textureResourcesName))
				{
					var skyboxMat = Resources.Load<Material>(settings.textureResourcesName);
					var newSB = vrcamera.gameObject.AddComponent<Skybox>();
					newSB.material = skyboxMat;
				}

				//camera.fieldOfView = settings.fieldOfView;
			}
		}

		[ContextMenu("allow temporarily to modify settings")]
		private void allowCameraModification()
		{
			SetHideflagsRecursive(_currentCamera.gameObject, hideFlagsToUse);
		}

		[ContextMenu("Get sample camera settings")]
		public void PrintCameraSettings()
		{
			var camera = _currentCamera.GetComponentInChildren<Camera>();
			var settings = new SerializedCameraSettings
			{
				clearFlags = camera.clearFlags,
				depth = camera.depth,
				farClipPlane = camera.farClipPlane
				//fieldOfView = camera.fieldOfView
			};
			settings.SaveCameraBackgroundColor(camera.backgroundColor);
			Debug.Log(settings.JsonSerialize());
		}

		private void destroyCamera()
		{
			if(_currentCamera && _currentCamera != null)
			{
				var cameraToDestroy = _currentCamera;
				_currentCamera = null;
				if(cameraToDestroy.activeInHierarchy)
				{
					DestroyImmediateRecursive(cameraToDestroy);
					//if we're shutting down, don't worry about it.  otherwise, kill it
				}
			}
		}

		public static void DestroyImmediateRecursive(GameObject go)
		{
			try
			{
				foreach(Transform child in go.transform)
				{
					DestroyImmediateRecursive(child.gameObject);
				}
				DestroyImmediate(go);
			}
			catch(Exception e)
			{
				Debug.Log("Cannot destroy camera child due to :" + e);
			}
		}

		public static void SetHideflagsRecursive(GameObject go, HideFlags flags)
		{
			foreach(Transform child in go.transform)
			{
				SetHideflagsRecursive(child.gameObject, flags);
			}
			go.hideFlags = flags;
		}

	}
}
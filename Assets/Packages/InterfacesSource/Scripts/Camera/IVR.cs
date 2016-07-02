using System;
using UnityEngine;

namespace interfaces
{
	public interface IVR
	{
		GameObject CreateCamera();
		CameraSettingsSO GetCameraSettingsSO();
		void SetCustomCameraSettings(string settingString);
		//void SetCameraSettings(SerializedCameraSettings settingString);

		//hacky thing for now to allow me to look for all camerasettings in the project and exclude one, or alternatively just grab this, since it should be on the nose
		//IEnumerator DoBlink();
		Transform LeftEyeCamera();
		Transform RightEyeCamera();
	}

	//allow me to set per-application or per-mode settings on the cameras without direct access
	//http://docs.unity3d.com/Manual/class-Camera.html for all possible fields I may want too use in the future
	[Serializable]
	public class SerializedCameraSettings
	{
		public float[] backgroundColor;
		public CameraClearFlags clearFlags;
		public string customCameraSettings; //ie flipy for oculus vr camera
		public float depth;
		//public float fieldOfView;
		public float farClipPlane;
		public string textureResourcesName;

		public Color getCameraBackgroundColor()
		{
			return backgroundColor == null
				? Color.white
				: new Color(backgroundColor[0], backgroundColor[1], backgroundColor[2], backgroundColor[3]);
		}

		public void SaveCameraBackgroundColor(Color color)
		{
			backgroundColor = new[] { color.r, color.g, color.b, color.a };
		}
	}

	public class CameraUtility
	{
		public static void SetCameraSettingsOnAllCameras(SerializedCameraSettings settings, GameObject parent)
		{
			var cameras = parent.GetComponentsInChildren<Camera>();
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
	}
}
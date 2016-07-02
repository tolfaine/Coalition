using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class CameraLoader
	{
		private readonly string _prefabName;
		private GameObject _cameraRef;

		public CameraLoader(string prefabName)
		{
			_prefabName = prefabName;
		}

		public GameObject CameraRef
		{
			get { return _cameraRef; }
		}

		public CameraSettingsSO CameraSettings
		{
			get { return Resources.Load<CameraSettingsSO>(_prefabName); }
		}

		public GameObject CreateCamera()
		{
			if(_cameraRef != null)
				return _cameraRef;
			_cameraRef = CameraSettings.Create();
			return _cameraRef;
		}
	}

	//[Implements(typeof(IVR), InjectionBindingScope.CROSS_CONTEXT)]
	public class NullVR : IVR
	{
		private readonly CameraLoader _loader;

		public NullVR()
		{
			_loader = new CameraLoader("NullVR");
		}

		public CameraLoader CameraLoader
		{
			get { return _loader; }
		}

		public GameObject CreateCamera()
		{
			return _loader.CreateCamera();
		}

		public CameraSettingsSO GetCameraSettingsSO()
		{
			return _loader.CameraSettings;
		}

		public void SetCustomCameraSettings(string settingString)
		{
		}

		public Transform LeftEyeCamera()
		{
			CameraLoader.CreateCamera();
			return CameraLoader.CameraRef.GetComponent<Camera>().transform;
		}

		public Transform RightEyeCamera()
		{
			CameraLoader.CreateCamera();
			return CameraLoader.CameraRef.GetComponent<Camera>().transform;
		}

		public IEnumerator DoBlink()
		{
			return null;
		}
	}
}
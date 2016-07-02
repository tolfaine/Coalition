using interfaces;
using System.Collections;
using UnityEngine;

namespace implementations
{
	[Implements(typeof(IVR))]
	public class DiveVR : IVR
	{
		private Camera _leftCamera;
		private Camera _rightCamera;
		private CameraLoader _loader;

		public DiveVR()
		{
			_loader = new CameraLoader("DiveVR");
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
			_loader.CreateCamera();
			if(_leftCamera == null)
				_leftCamera = _loader.CameraRef.transform.FindChild("Camera_left").GetComponent<Camera>();
			return _leftCamera.transform;
		}

		public Transform RightEyeCamera()
		{
			_loader.CreateCamera();
			if(_rightCamera == null)
				_rightCamera = _loader.CameraRef.transform.FindChild("Camera_right").GetComponent<Camera>();
			return _rightCamera.transform;
		}
		public IEnumerator DoBlink()
		{
			yield return null;
		}
	}

}
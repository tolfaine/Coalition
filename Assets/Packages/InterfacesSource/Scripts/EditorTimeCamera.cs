using UnityEngine;

namespace interfaces
{
	[ExecuteInEditMode]
	public class EditorTimeCamera : MonoBehaviour
	{
		private GameObject _currentCamera;
		[SerializeField]
		private CameraSettingsSO EditorBuildPreviewType;
		public HideFlags hideFlagsToUse = HideFlags.DontSave;

		[ContextMenu("testEnable")]
		public void OnEnable()
		{
			if(Application.isPlaying)
			{
				enabled = false;
				return;
			}
			destroyCamera();

			//editor -- note: Returns true in the Unity editor when in play mode as well as Returns true when in any kind of player (Read Only).
			var vrCameraType = EditorBuildPreviewType;
			_currentCamera = vrCameraType.Create();
			var CAMERA_OFFSET_POSITION = new Vector3(0f, 0f, 0f);
			_currentCamera.name = vrCameraType + " Editor Only(Do not edit)";
			_currentCamera.transform.parent = transform;
			_currentCamera.transform.localPosition = CAMERA_OFFSET_POSITION;

			GameObjectExtensionMethods.SetHideflagsRecursive(this, hideFlagsToUse);
		}

		public void OnDisable()
		{
			destroyCamera();
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
					GameObjectExtensionMethods.DestroyImmediateRecursive(this, cameraToDestroy);
					//if we're shutting down, don't worry about it.  otherwise, kill it
				}
			}
		}
	}
}
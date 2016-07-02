using UnityEngine;

namespace interfaces
{
	public class MoveUnderneathCamera
	{
		private Transform _originalParent;

		public void MoveInCenter(GameObject toMove,Vector3 localOffsetFromCamera)
		{
			_originalParent = toMove.transform.parent;

			var cameraFactory = Object.FindObjectOfType<VRCameraFactory>();
			var cameraGO = cameraFactory.GetCurrentCamera();

			//oculus workaround
			var oculusCenterEyeAnchor = cameraGO.transform.FindChild("CenterEyeAnchor");
			if(oculusCenterEyeAnchor != null)
				cameraGO = oculusCenterEyeAnchor.gameObject;
			toMove.transform.parent = cameraGO.transform;
			toMove.transform.localEulerAngles = Vector3.zero;
			toMove.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			toMove.transform.localPosition = localOffsetFromCamera; //+ new Vector3(0, 0, 1f);
		}

		public void ResetToOriginal(GameObject toMove)
		{
			toMove.transform.parent = _originalParent;
		}
	}
}
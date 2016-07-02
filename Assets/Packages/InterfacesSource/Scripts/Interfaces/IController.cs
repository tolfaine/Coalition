using System;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	//NOTE: keep an eye on the "capabilities" when it comes to implementation time
	//refactor the whole damn thing if things like hand position become too onerous
	// but for a simple use-case, it wouldn't be the end of the world if the interface went into a modal dialog and then changed movement into hand position or something
	public interface IController
	{
		List<UnityDefaultInputAxisController.Capabilities> capabilities { get; }

		bool GetButton(Control.ControlName control);

		Vector2 MovementDelta();

		Vector2 RotationDelta();

		HandsInfo GetHandPosition();

		GameObject GameObjectController();
	}

	//starting with xbox style controls in the abstract and then will add any vr specifics
	//ultimately, the input control for hydra, leap, etc should be the same as the lowest common denominator -- xbox controler or keyboard
	public enum ControlType
	{
		AXIS,
		BUTTON
	}

	public class Control
	{
		public enum ControlName
		{
			MOVEMENT,
			HAND,
			BUTTON_ACTION1,
			BUTTON_ACTION2,
			BUTTON_ACTION3,
			BUTTON_CANCEL
		}

		public ControlName control;
		public ControlType type;
	}

	//in leu of testing, I want to see if this makes *any* sense before diving into implementations for leap, etc
	//testing and sample implementations are a good way to vet interface design before going too far (hah.. famous last words)
	//this uses just raw unity input for now using the deafault settings I got in the project
	public class UnityDefaultInputAxisController : IController
	{
		public enum Capabilities
		{
			HAND_TRACKING,
			BUTTONS
		}

		private readonly List<Capabilities> _capabilities = new List<Capabilities> { Capabilities.BUTTONS };

		private readonly Dictionary<Control.ControlName, Func<bool>> buttonMap =
			new Dictionary<Control.ControlName, Func<bool>>
		{
			{Control.ControlName.BUTTON_ACTION1, () => buttonPressed("Fire1")},
			{Control.ControlName.BUTTON_ACTION2, () => buttonPressed("Fire2")},
			{Control.ControlName.BUTTON_ACTION3, () => buttonPressed("Fire3")},
			{Control.ControlName.BUTTON_CANCEL, () => buttonPressed("Cancel")}
		};

		public List<Capabilities> capabilities
		{
			get { return _capabilities; }
		}

		public bool GetButton(Control.ControlName control)
		{
			if(!buttonMap.ContainsKey(control))
				throw new NotSupportedException("button not available:" + control);
			var functionToGetControlOutput = buttonMap[control];
			return functionToGetControlOutput();
		}

		public Vector2 MovementDelta()
		{
			return new Vector2();
		}

		public Vector2 RotationDelta()
		{
			return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		}

		public HandsInfo GetHandPosition()
		{
			throw new NotSupportedException("Hand tracking not supported -- make sure to check capabilities");
		}

		public GameObject GameObjectController()
		{
			return null;
		}

		public Vector2 GetMovementDelta()
		{
			return new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
		}

		private static bool buttonPressed(string buttonName)
		{
			return Input.GetButtonDown(buttonName);
		}
	}

	public class HandsInfo
	{
		public HandInfo left;
		public HandInfo right;
	}

	public class HandInfo
	{
		public float grabStrength;
		public Vector3 palmNormal;
		public Vector3 palmPos;
		public Vector3 palmVelocity;
		public float pinchStrength;
	}
}
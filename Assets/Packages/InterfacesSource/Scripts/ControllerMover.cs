using System;
using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class ControllerMover : MonoBehaviour
	{
		private IController _controller;

		//make sure to do instant-ish movement and keep velocity constant.  Do this by modifying the input from the user, and decaying it
		private float _movementForceFromUser;

		private Rigidbody _myRigidbody;

		[SerializeField]
		private float targetVelocity;

		[SerializeField]
		private readonly float convienienceModeTurningAngle = 32.5f;

		private IEnumerator _pollForInputEnumerator; //so I can stop it explicitly if I want, check if its running
		private IEnumerator _handleMovement;
		public void Init(IController controller)
		{
			_controller = controller;
			_myRigidbody = GetComponent<Rigidbody>();
			if(_myRigidbody)
			{
				throw new Exception("need a rigidbody on the controller mover view");
			}
			if(_pollForInputEnumerator == null)
			{
				_pollForInputEnumerator = pollForStandardControls();
				StartCoroutine(_pollForInputEnumerator);
			}
			if(_handleMovement == null)
			{
				_handleMovement = handleMovement();
				StartCoroutine(_handleMovement);
			}
		}
		public void convienienceTurnRight()
		{
		}

		private IEnumerator handleMovement()
		{
			const float minimumForceForMovement = 0.01f;
			while(true)
			{
				//Debug.Log("Movement from user:" + _movementForceFromUser);
				if(Mathf.Abs(_movementForceFromUser) > minimumForceForMovement)
				{
					//Vector3 velocityVector = _myRigidbody.velocity;
					//we want to keep the absolute velocity constant, even if we've moving forward
					//_myRigidbody.velocity = _myRigidbody.
					//velocityVector.Normalize();
					//TODO: see if I should, when turning, lerp or something between old velocity vector and new one?
					//that would lead to skidding, etc
					
					_myRigidbody.velocity = transform.forward * targetVelocity * (_movementForceFromUser > 0 ? 1f : -1f);
					//Debug.Log(string.Format("Movement velocity: {0} forward vector:{1} targetVelocity:{2}", _myRigidbody.velocity,
					//	transform.forward, targetVelocity));
				}
				else
					_myRigidbody.velocity = Vector3.zero;
				_movementForceFromUser *= .5f; //decay user input
				yield return null;
			}
		}

		private IEnumerator pollForStandardControls()
		{
			const float minimumSquareMagnitudeForMovement = 0.01f;
			while(true)
			{
				if(_controller.GetButton(Control.ControlName.BUTTON_ACTION3))
					transform.Rotate(Vector3.up * convienienceModeTurningAngle);
				if(_controller.GetButton(Control.ControlName.BUTTON_ACTION2))
					transform.Rotate(Vector3.down * convienienceModeTurningAngle);

				var movementDelta = _controller.MovementDelta();
				//Debug.Log("movement delta:" + movementDelta);
				if(movementDelta.sqrMagnitude > minimumSquareMagnitudeForMovement)
				{
					_movementForceFromUser += movementDelta.x;
					//TODO: see how to handle deadzone stuff like this. Clarify if it should be handled in the controller instead, which seems more appropriate
					if(Mathf.Abs(movementDelta.y) > 0.1f)
						transform.Rotate(Vector3.up * movementDelta.y);
				}

				yield return null;
				//TODO: if the controller is handled on OnFixedUpdate, change this... maybe have a controllerCapabilities? or a poll interval coroutine?
			}
		}
	}
}
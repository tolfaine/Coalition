using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class CursorController : MonoBehaviour
	{
		[SerializeField]
		private GameObject _cursor;

		private MoveUnderneathCamera _cursorMover;
		private SpriteRenderer _myRenderer;
		public Sprite RegularSprite;
		public Sprite SpinSprite;

		public void Start()
		{
			_cursorMover = new MoveUnderneathCamera();
			_myRenderer = _cursor.GetComponent<SpriteRenderer>();

			_myRenderer.enabled = false;
		}

		[ContextMenu("Show cursor")]
		public void ShowCursor()
		{
			_cursorMover.MoveInCenter(_cursor,Vector3.forward);
			_myRenderer.enabled = true;
		}

		[ContextMenu("Hide cursor")]
		public void HideCursor()
		{
			_cursorMover.ResetToOriginal(_cursor);
			_myRenderer.enabled = false;
		}

		[ContextMenu("Spin Cursor")]
		public void Spin()
		{
			SpinCursor(10f);
		}

		public void SpinCursor(float sequenceRunTime)
		{
			StartCoroutine(DoSpin(sequenceRunTime));
		}

		private IEnumerator DoSpin(float time)
		{
			_myRenderer.sprite = SpinSprite;
			ShowCursor();

			var originalScale = _cursor.transform.localScale;
			var originalRotation = _cursor.transform.localRotation;
			const float cycleTime = 1f;

			var timeElapsed = 0f;
			while(timeElapsed < time)
			{
				var normalizedTime = (timeElapsed % cycleTime) / cycleTime;
				if(normalizedTime > 0.5f)
					_cursor.transform.localScale = Mathf.Lerp(1f, 2f, normalizedTime * 2f) * Vector3.one;
				else
				{
					var normalizedTimeOverHalf = normalizedTime - (normalizedTime / 2f);
					_cursor.transform.localScale = Mathf.Lerp(2f, 1f, normalizedTimeOverHalf * 2f) * Vector3.one;
				}
				_cursor.transform.Rotate(Vector3.forward * Time.deltaTime * 40f);
				yield return null;
				timeElapsed += Time.deltaTime;
			}
			_cursor.transform.localScale = originalScale;
			_cursor.transform.localRotation = originalRotation;

			_myRenderer.sprite = RegularSprite;
			HideCursor();
		}
	}
}
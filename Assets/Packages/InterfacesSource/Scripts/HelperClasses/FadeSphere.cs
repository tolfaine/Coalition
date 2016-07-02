using System.Collections;
using UnityEngine;

namespace interfaces
{
	public class FadeSphere : MonoBehaviour
	{
		[ContextMenu("test fade")]
		public void testFade()
		{
			StartCoroutine(Fade(3f));
		}

		[ContextMenu("full blink")]
		public void doFullBlink()
		{
			StartCoroutine(FullBlink((3f)));
		}
		private void SetMaterialAlpha(Renderer display, float a)
		{
			Color black = Color.black;
			black.a = a;
			display.material.SetColor("_Color", black);
		}

		public IEnumerator FullBlink(float totalTimeToRun)
		{
			yield return StartCoroutine(Fade(totalTimeToRun / 2f));
			yield return StartCoroutine(Fade(totalTimeToRun / 2f, true));
		}

		public IEnumerator Fade(float totalTimeToRun, bool startBlack = false)
		{
			var mover = new MoveUnderneathCamera();
			mover.MoveInCenter(gameObject,Vector3.zero);

			var display = GetComponent<Renderer>();

			display.enabled = true;
			var elapsedTime = 0f;
			while(elapsedTime < totalTimeToRun)
			{	
				float alphaVal = Mathf.Lerp(0, 1, (elapsedTime / totalTimeToRun));
				if(startBlack)
				{
					alphaVal = 1 - alphaVal;
				}
				SetMaterialAlpha(display, alphaVal);
				//display.material.SetColor("_Color", color);
				yield return null;
				elapsedTime += Time.deltaTime;
			}

			display.enabled = false;
			mover.ResetToOriginal(gameObject);
		}
	}
}
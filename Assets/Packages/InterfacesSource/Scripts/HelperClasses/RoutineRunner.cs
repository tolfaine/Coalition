using System.Collections;
using UnityEngine;

namespace interfaces
{
	public interface IRoutineRunner
	{
		Coroutine StartCoroutine(IEnumerator toRun);
	}

	[Implements(typeof(IRoutineRunner))]
	public class RoutineRunner : MonoBehaviour, IRoutineRunner
	{
	}
}
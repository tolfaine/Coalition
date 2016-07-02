using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	public interface IVoiceControlService
	{
		void Init();

		IEnumerator SendAudio(AudioClip clip, List<string> resultWords);
	}
}
using System.Collections;
using UnityEngine;

namespace interfaces
{
	public interface ISoundPlayer
	{
		float PlaySound(string soundName);
		float PlaySound(string soundName, Vector3 position);
		void StopSound(string soundName);
		void StopGroup(SoundGroup group);
		void StopGroup(string group);
		void SetGlobalVolume(float weight);
		float GetGlobalVolume();
		void SetGroupVolume(SoundGroup group, float weight); // between 0 and 1f
		float GetGroupVolume(SoundGroup group);
		IEnumerator Init();
		void Dispose();
	}

	public enum SoundGroup
	{
		SFX,
		MUSIC,
		GUI,
		//guessing at others...
		ENEMIES,
		PLAYER
	}
}
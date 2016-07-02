using UnityEngine;

namespace interfaces
{
	public interface IMoviePlayer
	{
		void Load(string mediaName, GameObject targetMaterial, bool loop, bool autoPlay);
		MoviePlayerState GetCurrentState();
		bool Play(float time);
	}

	public enum MoviePlayerState
	{
		UNKNOWN,
		PLAYING
	}

}

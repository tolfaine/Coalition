//libraries need to be able to subscribe to lifecycle messages without creating a new thing sometimes
//specifically, the common cases are awake, OnLevelLoaded, OnApplicationPause and OnApplicationQuit

using System;
using UnityEngine;

namespace interfaces
{
	public interface IMonobehaviourLifecycleMessages
	{
		event Action AwakeAction;

		event Action StartAction;

		//event Action OnEnableAction;
		//event Action OnDisableAction;
		event Action OnLevelWasLoadedAction;

		event Action OnApplicationQuitAction;

		event Action OnApplicationPauseAction; //(bool paused);

		event Action OnApplicationUnPauseAction; //(bool paused);
		event Action OnConnectedToServerAction;
		event Action<NetworkConnectionError> OnFailedToConnectAction;
		event Action OnServerInitializedAction;

		event Action<NetworkPlayer> OnPlayerConnectedAction;
		event Action OnPlayerDisconnectedAction;
	}
}
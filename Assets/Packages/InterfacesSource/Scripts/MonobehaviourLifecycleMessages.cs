using System;
using UnityEngine;

namespace interfaces
{
	public class MonobehaviourLifecycleMessages : MonoBehaviour, IMonobehaviourLifecycleMessages
	{
		public event Action AwakeAction;

		public event Action StartAction;

		public event Action OnLevelWasLoadedAction;

		public event Action OnApplicationQuitAction;

		public event Action OnApplicationPauseAction;

		public event Action OnApplicationUnPauseAction;

		public event Action OnConnectedToServerAction;
		public event Action<NetworkConnectionError> OnFailedToConnectAction;
		public event Action OnServerInitializedAction;
		public event Action<NetworkPlayer> OnPlayerConnectedAction;
		public event Action OnPlayerDisconnectedAction;

		public void OnPlayerConnected(NetworkPlayer player)
		{
			if(OnPlayerConnectedAction != null)
			{
				OnPlayerConnectedAction(player);
			}
		}

		public void OnPlayerDisconnected()
		{
			if(OnPlayerDisconnectedAction != null)
			{
				OnPlayerDisconnectedAction();
			}
		}

		public void Awake()
		{
			if(AwakeAction != null)
				AwakeAction();
		}

		public void OnConnectedToServer()
		{
			if(OnConnectedToServerAction != null)
			{
				OnConnectedToServerAction();
			}
		}
		public void OnFailedToConnect(NetworkConnectionError error)
		{
			if(OnFailedToConnectAction != null)
			{
				OnFailedToConnectAction(error);
			}
		}
		public void OnServerInitialized()
		{
			if(OnServerInitializedAction != null)
			{
				OnServerInitializedAction();
			}
		}
		public void Start()
		{
			if(StartAction != null)
				StartAction();
		}

		public void OnLevelWasLoaded()
		{
			if(OnLevelWasLoadedAction != null)
				OnLevelWasLoadedAction();
		}

		public void OnApplicationQuit()
		{
			if(OnApplicationQuitAction != null)
				OnApplicationQuitAction();
		}

		public void OnApplicationPause(bool paused)
		{
			if(paused)
			{
				if(OnApplicationPauseAction != null)
					OnApplicationPauseAction();
			}
			else
			{
				if(OnApplicationUnPauseAction != null)
					OnApplicationUnPauseAction();
			}
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	[Serializable]
	public class SerializedDictionary<T, V> : ISerializationCallbackReceiver
	{
		private readonly Dictionary<T, V> _runtimeDictionary = new Dictionary<T, V>();
		private readonly List<Tuple<T, V>> _serializedDictionary = new List<Tuple<T, V>>();

		public V this[T index]
		{
			get { return _runtimeDictionary[index]; }
			set { _runtimeDictionary[index] = value; }
		}

		public void OnBeforeSerialize()
		{
			_serializedDictionary.Clear();
			foreach(var keyValuePair in _runtimeDictionary)
			{
				_serializedDictionary.Add(new Tuple<T, V> { first = keyValuePair.Key, second = keyValuePair.Value });
			}
			//will throw an error if multiple keys are duplicated
		}

		public void OnAfterDeserialize()
		{
			_runtimeDictionary.Clear();
			foreach(var kvp in _serializedDictionary)
			{
				_runtimeDictionary.Add(kvp.first, kvp.second);
			}
		}

		public bool ContainsKey(T index)
		{
			return _runtimeDictionary.ContainsKey(index);
		}

		public void Add(T key, V value)
		{
			_runtimeDictionary.Add(key, value);
		}
	}

	[Serializable]
	public class Tuple<T, V>
	{
		public T first;

		public V second;
	}
}
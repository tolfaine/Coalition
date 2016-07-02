namespace interfaces
{
	public class JsonFxJson : JsonSerializer
	{
		public string Serialize(object toSerialize)
		{
			return toSerialize.JsonSerialize();
		}

		public T Deserialize<T>(string input)
		{
			return input.JsonDeserialize<T>();
			//return default(T);
		}
	}
}
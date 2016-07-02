namespace interfaces
{
	public interface JsonSerializer
	{
		string Serialize(object toSerialize);

		T Deserialize<T>(string input);
	}
}
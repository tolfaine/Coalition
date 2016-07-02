using Pathfinding.Serialization.JsonFx;

public static class JsonExtensions
{
	public static string JsonSerialize(this object toSerialize)
	{
		return JsonWriter.Serialize(toSerialize);
	}

	public static T JsonDeserialize<T>(this string serializedVersion)
	{
		return JsonReader.Deserialize<T>(serializedVersion);
	}
}
using System.Collections.Generic;
using UnityEngine;

namespace interfaces
{
	public interface IResourceLoader
	{
		T GetAssetByPath<T>(string path) where T : Object;

		List<T> GetAllAssetsInFolder<T>(string path) where T : Object;
	}

	[Implements(typeof(IResourceLoader))]
	public class ResourceLoader : IResourceLoader
	{
		public static T Load<T>() where T : Object
		{
			string name = typeof(T).Name;
			return Resources.Load<T>(name);
		}

		public T GetAssetByPath<T>(string path) where T : Object
		{
			return Resources.Load<T>(path);
		}

		public List<T> GetAllAssetsInFolder<T>(string pathIn) where T : Object
		{
			string path = pathIn;
			//apparently if we try to LoadAll, and the path does not end with a seperator char, it is not interpreted as a directory... so add a slash if there isn't one
			if(!path.EndsWith("/"))
				path += "/";
			var ToReturn = new List<T>();
			T[] resources = Resources.LoadAll<T>(path);
			if(resources == null)
			{
				path = "Resources/" + path;
				resources = Resources.LoadAll<T>(path);
			}
			ToReturn.AddRange(resources);
			return ToReturn;
		}
	}
}
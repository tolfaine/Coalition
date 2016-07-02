using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace interfaces
{
	public class ReflectionHelpers
	{
		/*
		public Type[] GetTypesThatImplementMembers(string interfaceName)
		{
		}
		*/

		// a little heavy handed going through *all* assemblies, but whatever
		//the approach that was taken in the strange assembly loader is probably best
		//either a magic string "_asink" and the unity default "Assembly-" prefix
		//best optimization is to probably pass a list in
		public Type GetTypeInAssemblies(string typeName)
		{
			//note: could use isAssigableFrom, but just a straight name check sorta works.  if it blows up, that's ok
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				//assembly.CreateInstance()
				//assembly.GetTypes()
				//var tempClass = assembly.GetType(typeName);
				//this method gets the
				IEnumerable<Type> tempClass = assembly.GetTypes().Where(x => x.Name == typeName);
				if(tempClass != null)
				{
					//var enumerator = tempClass.GetEnumerator();
					foreach(var type in tempClass)
					{
						return type;
					}
				}
			}
			throw new Exception("Type not found in binding:" + typeName);
		}

		public Dictionary<string, Type> GetTypesInAssemblies(List<string> types)
		{
			var dic = new Dictionary<string, Type>();
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				//Type[] typesInAssembly = assembly.GetTypes();
				//NOTE: this means that if two types are picked up, then the second will be ignored
				foreach(string typeName in types)
				{
					if(dic.ContainsKey(typeName))
					{
						Debug.LogWarning(string.Format("type {0} had multiple entries in several assemblies, this is probably bad", typeName));
						continue;
					}

					Type testValue = assembly.GetType(typeName, false, true);
					if(testValue == null)
						continue;
					dic.Add(typeName, testValue);
				}
			}

			return dic;
		}
	}

	public class DefaultImplementationAttribute : Attribute
	{
	}
}
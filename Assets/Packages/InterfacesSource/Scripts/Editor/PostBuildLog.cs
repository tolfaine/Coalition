/* **************************************************************************
   Copyright 2012 Calvin Rien
   (http://the.darktable.com)

   Derived from a method in BuildManager, part of
   VoxelBoy's Unite 2012 Advanced Editor Scripting Talk.
   (http://bit.ly/EditorScripting)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

************************************************************************** */

//from https://gist.github.com/darktable/3505922
//Unity3D: Post-process script that includes a build.log file containing the build summary in the output directory. Based on a method in BuildManager.
//changed to have a targetName_build.log
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PostBuildLog : ScriptableObject
{
	[PostProcessBuild] // Requires Unity 3.5+
	private static void OnPostProcessBuildPlayer(BuildTarget target, string buildPath)
	{
		WriteBuildLog(buildPath, target.ToString());
	}

	public static void WriteBuildLog(string buildPath, string target = "")
	{
		string editorLogFilePath = null;
		string[] pieces = null;

		bool winEditor = Application.platform == RuntimePlatform.WindowsEditor;

		if(winEditor)
		{
			editorLogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			pieces = new string[] { "Unity", "Editor", "Editor.log" };
		}
		else
		{
			editorLogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			pieces = new string[] { "Library", "Logs", "Unity", "Editor.log" };
		}

		foreach(var e in pieces)
		{
			editorLogFilePath = Path.Combine(editorLogFilePath, e);
		}

		if(!File.Exists(editorLogFilePath))
		{
			Debug.LogWarning("Editor log file could not be found at: " + editorLogFilePath);
			return;
		}

		StringBuilder report = new StringBuilder();

		using(StreamReader reader = new StreamReader(File.Open(editorLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
		{
			while(true)
			{
				string line = reader.ReadLine();
				if(line == null)
				{
					break;
				}

				if(line.StartsWith("Mono dependencies included in the build"))
				{
					report.Length = 0;
					report.AppendFormat("Build Report @ {0}\n\n", System.DateTime.Now.ToString("u"));
				}

				if(report != null)
				{
					report.AppendLine(line);
				}
			}
		}

		if(report.Length == 0)
		{
			return; // No builds have been run.
		}

		string outputPath = null;
		
		if(target.StartsWith("Standalone") || target.Contains("Android"))
		{
			outputPath = Path.Combine(Path.GetDirectoryName(buildPath), "build.log");
		}
		else
		{
			outputPath = Path.Combine(buildPath, "build.log");
		}

		FileInfo buildLogFile = new FileInfo(outputPath);
		string duplicateBuildLogOutputPath = outputPath.Replace("build.log", "build" + target + ".log");
		FileInfo duplicateFile = new FileInfo(duplicateBuildLogOutputPath);
		try
		{
			using(StreamWriter writer = buildLogFile.CreateText())
			{
				writer.Write(report.ToString());
			}
			using(StreamWriter writer = duplicateFile.CreateText())
			{
				writer.Write(report.ToString());
			}
		}
		catch
		{
			Debug.LogError("Build log file could not be created for writing at: " + buildLogFile.FullName);
		}
	}
}
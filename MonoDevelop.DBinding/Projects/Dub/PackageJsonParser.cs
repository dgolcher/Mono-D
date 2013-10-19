﻿using MonoDevelop.Core;
using MonoDevelop.Projects.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using MonoDevelop.Projects;

namespace MonoDevelop.D.Projects.Dub
{
	public class PackageJsonParser : IFileFormat
	{
		public bool CanReadFile(FilePath file, Type expectedObjectType)
		{
			return file.FileName == "package.json" &&
				(expectedObjectType.Equals(typeof(WorkspaceItem)) ||
				expectedObjectType.Equals(typeof(SolutionEntityItem)));
		}

		public bool CanWriteFile(object obj)
		{
			return false; // Everything has to be manipulated manually (atm)!
		}

		public void ConvertToFormat(object obj)
		{
			
		}

		public IEnumerable<string> GetCompatibilityWarnings(object obj)
		{
			yield return string.Empty;
		}

		public List<FilePath> GetItemFiles(object obj)
		{
			return new List<FilePath>();
		}

		public Core.FilePath GetValidFormatName(object obj, Core.FilePath fileName)
		{
			return fileName.ParentDirectory.Combine("package.json");
		}

		DubSolution readSolution;
		DateTime lastReadTime;

		public object ReadFile(FilePath file, Type expectedType, IProgressMonitor monitor)
		{
			var writeTime = File.GetLastWriteTimeUtc (file);
			if (readSolution == null || lastReadTime < writeTime) {
				if (readSolution != null)
					readSolution = null;
				using (var s = File.OpenText (file))
				using (var r = new JsonTextReader (s))
					readSolution = ReadPackageInformation (file, r);
				lastReadTime = writeTime;
			}

			if (expectedType.Equals (typeof(SolutionEntityItem)))
				return readSolution.StartupItem;
			else if (expectedType.Equals (typeof(WorkspaceItem)))
				return readSolution;

			return null;
		}

		public static DubSolution ReadPackageInformation(FilePath packageJsonPath,JsonReader r)
		{
			var sln = new DubSolution (packageJsonPath);

			var defaultPackage = new DubProject();
			defaultPackage.FileName = packageJsonPath;
			defaultPackage.BaseDirectory = packageJsonPath.ParentDirectory;

			sln.RootFolder.AddItem (defaultPackage, false);
			sln.StartupItem = defaultPackage;


			while (r.Read ()) {
				if (r.TokenType == JsonToken.PropertyName) {
					var propName = r.Value as string;
					defaultPackage.TryPopulateProperty (propName, r);
				}
				else if (r.TokenType == JsonToken.EndObject)
					break;
			}

			defaultPackage.AddProjectAndSolutionConfiguration(new DubProjectConfiguration { Name = "Default", Id = "Default" });
			defaultPackage.UpdateFilelist ();

			sln.LoadUserProperties ();

			return sln;
		}

		public bool SupportsFramework(Core.Assemblies.TargetFramework framework)
		{
			return false;
		}

		public bool SupportsMixedFormats
		{
			get { return true; }
		}

		public void WriteFile(Core.FilePath file, object obj, Core.IProgressMonitor monitor)
		{
			monitor.ReportError ("Can't write dub package information! Change it manually in the definition file!", new InvalidOperationException ());
		}
	}
}
using Godot;
using Godot.Collections;
using SavedPassword;
using NewConsole;
public static class FileAccessSystem {

	private const string folderName = "/SaveData/";

	/// <summary>
/// Gets the global location of the executable file
/// </summary>
/// <returns></returns>
	public static string GetCurrentLocation() {

		Godot.Directory dir = new Godot.Directory();
		dir.Open(OS.GetExecutablePath()+"/..");
		return dir.GetCurrentDir();
	}

	/// <summary>
	/// Write current project settings to override file
	/// </summary>
	public static void WriteSettings() {

		if(!Settings.safeMode) {

			Debugger.Print("Saving Project Settings");
			ProjectSettings.SaveCustom("override.cfg");
		}
	}

	/// <summary>
	/// Write a list of passwords to file
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="data"></param>
	public static void WriteAllUserData(string fileName, Godot.Collections.Array<PasswordData> data) {

		if(!Settings.safeMode) {

			string targetFile = GetCurrentLocation() + folderName + fileName;

			File file = new File();

			if(!FolderExists(folderName)) {

				Debugger.Print("Folder '" + folderName + "' Doesnt Exist", Debugger.DebuggerState.STATE_WARNING);
				Debugger.Print("Creating New Folder");
				CreateFolder(GetCurrentLocation() + folderName);
			}

			if(data.Count > 0) {

				Debugger.Print("Attempting To Open File: " + targetFile);
				file.Open(targetFile, File.ModeFlags.Write);
				foreach(PasswordData password in data) {

					file.StoreLine(JSON.Print(password.Save()));
				}
				file.Close();
			}
			else {

				Debugger.Print("File Empty: " + targetFile);
				file.Open(targetFile, File.ModeFlags.Write);
				file.Close();
			}
		}
	}

	/// <summary>
	/// Write a single password to the end of file
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="data"></param>
	public static void WritePassword(string fileName, PasswordData data) {

		if(!Settings.safeMode) {

			string targetFile = GetCurrentLocation() + folderName + fileName;

			if(!FolderExists(folderName)) {

				Debugger.Print("Folder '" + folderName + "' Doesnt Exist", Debugger.DebuggerState.STATE_WARNING);
				Debugger.Print("Creating New Folder");
				CreateFolder(GetCurrentLocation() + folderName);
			}

			File file = new File();
			file.Open(targetFile, File.ModeFlags.Write);

			Debugger.Print("Writing '" + data.Id + "' To File");
			file.StoreLine(JSON.Print(data.Save()));
			file.Close();
		}
	}

	public static void WriteStringToUser(string fileName, string data) {

		if(!Settings.safeMode) {

			File file = new File();
			file.Open("user://" + fileName, File.ModeFlags.Write);
			file.StoreString(data);
			file.Close();
		}
	}
	public static string ReadStringFromUser(string fileName) {

		File file = new File();
		switch(file.Open("user://" + fileName, File.ModeFlags.Read)) {

			case Error.Ok:
				return file.GetAsText();

			case Error.FileNotFound:
				throw new FileNotFoundException("user://" + fileName);
		}
		return "";
	}

	/// <summary>
	/// Read the current saved passwords from file
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	/// <exception cref="FileNotFoundException"></exception>
	public static Godot.Collections.Array<PasswordData> ReadUserData(string fileName) {

		Godot.Collections.Array<PasswordData> array = new Godot.Collections.Array<PasswordData>();

		Godot.File file = new Godot.File();
		string folderLocation = GetCurrentLocation();

		Debugger.Print("Opening File: '"+fileName+"'");
		switch(file.Open(folderLocation + folderName + fileName, Godot.File.ModeFlags.Read)) {

			case Error.Ok:

				Debugger.Print("File Opened");
				while(file.GetPosition() < file.GetLen()) {
					
					Godot.Collections.Dictionary dictionary = (Godot.Collections.Dictionary) JSON.Parse(file.GetLine()).Result;
					array.Add(new PasswordData(
						(string) dictionary["key"],
						newLabel: (string) dictionary["label"],
						newGroup: (string) dictionary["group"],
						newLegacy: bool.Parse((string) dictionary["legacy"]),
						newCount: int.Parse((string) dictionary["count"]),
						newSet: (CharacterSets) int.Parse((string) dictionary["set"]),
						newInvalid: (string) dictionary["invalid"],
						newIndex: dictionary.Contains("index") ? int.Parse((string) dictionary["index"]) : 0
					));
				}
		

				Debugger.Print("End Of File");
				break;

			case Error.FileNotFound:

				throw new FileNotFoundException(fileName);
		}
		file.Close();
		return array;
	}

	/// <summary>
	/// Check if a folder exists
	/// </summary>
	/// <param name="targetFolder"></param>
	/// <returns></returns>
	private static bool FolderExists(string targetFolder) {

		Debugger.Print("Checking If Folder: '" + targetFolder + "' Exists");
		Directory dir = new Directory();
		return dir.DirExists(targetFolder);
	}

	/// <summary>
	/// Create a nw folder directory
	/// </summary>
	/// <param name="targetFolder"></param>
	private static void CreateFolder(string targetFolder) {

		Debugger.Print("Creating Folder: "+targetFolder);
		Directory dir = new Directory();
		dir.MakeDirRecursive(targetFolder);
	}
}
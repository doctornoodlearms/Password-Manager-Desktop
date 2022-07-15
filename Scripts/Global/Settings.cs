using Godot;
using Godot.Collections;
using System;
using System.Reflection;
using SavedPassword;
using NewConsole;

public class Settings : Node{

	public static Boolean hideMaster {

		get => (Boolean) ProjectSettings.GetSetting("application/config/HideMaster");
		set => UpdateSetting("config/HideMaster", value);
	}
	public static Int32 characterCount {

		get => (Int32) ProjectSettings.GetSetting("application/config/CharacterCount");
		private set => UpdateSetting("config/CharacterCount", value);
	}
	public static String unusableCharacters {
		get => (String) ProjectSettings.GetSetting("application/config/InvalidCharacters");
		set => UpdateSetting("config/InvalidCharacters", value);
	}
	public static Int32 characterSet {
		get => (Int32) ProjectSettings.GetSetting("application/config/CharacterSet");
		set => UpdateSetting("config/CharacterSet", value);
	}
	public static String master {

		get => ((String) ProjectSettings.GetSetting("application/secret/Master"));
		set => SetMaster(value);
	}
	public static Boolean saveMaster {

		get => (Boolean) ProjectSettings.GetSetting("application/config/SaveMaster");
		set => SetSaveMaster(value);
	}
	public static String token {

		get => (String) ProjectSettings.GetSetting("application/secret/Token");
		set => UpdateSetting("secret/Token",value);
	}
	public static Boolean legacy {

		get => (Boolean) ProjectSettings.GetSetting("application/config/LegacyAlgorithm");
		set => UpdateSetting("config/LegacyAlgorithm", value);
	}
	public static bool safeMode {

		get;
		set;
	}
	public static string ProgramEnvVar {

		get => (string) ProjectSettings.GetSetting("application/config/ProgramEnviornmentVar");
		set => ProjectSettings.SetSetting("application/config/ProgramEnviornmentVar", value);
	}

	public static String GetCharacters() {

		Debugger.Print("Getting Character List");

		String usuableCharacters = "";
		String currentCharacterSet = "";

		Debugger.Print("Accounting For Character Set");

		switch((CharacterSets) characterSet) {

			case CharacterSets.SET_ALL:
				currentCharacterSet= "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ`1234567890-=[]\\;',./~!@#$%^&*()_+{}|:<>?";
				break;

			case CharacterSets.SET_ALPHANUMERIC:
				currentCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ`1234567890";
				break;

			case CharacterSets.SET_ALPHABET:
				currentCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ";
				break;
		}

		Debugger.Print("Accounting For Unusable Characters");
		foreach(Char i in currentCharacterSet) {

			if(unusableCharacters.Contains(i.ToString()) == false) {

				usuableCharacters += i;
			}
		}

		return usuableCharacters;
	}

	public static void SetCharacterCount(String value) {

		Debugger.Print("Updating Character Count");
		if(value.IsValidInteger()) {

			characterCount = value.ToInt();
		}
	}
	private static void SetMaster(String value) {

		Debugger.Print("Updating Master");
		UpdateSetting("secret/Master", value);
	}
	private static void SetSaveMaster(Boolean value) {

		UpdateSetting("config/SaveMaster", value);
	}
	private static void UpdateSetting(String settingName, System.Object value) {

		ProjectSettings.SetSetting("application/" + settingName, value);
	}
	public static void SaveToFile() {

		try {

			Debugger.Print("Saving Project Settings");
			FileAccessSystem.WriteSettings();
		}
		catch(FileNotFoundException e) {

			Debugger.Print(e.Message, Debugger.DebuggerState.STATE_ERROR);
		}
	}
}

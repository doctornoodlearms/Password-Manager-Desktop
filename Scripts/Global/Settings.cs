using Godot;
using Godot.Collections;
using SavedPassword;
using NewConsole;

public class Settings : Node{

	public static bool hideMaster {

		get => (bool) ProjectSettings.GetSetting("application/config/HideMaster");
		set => UpdateSetting("config/HideMaster", value);
	}
	public static int characterCount {

		get => (int) ProjectSettings.GetSetting("application/config/CharacterCount");
		private set => UpdateSetting("config/CharacterCount", value);
	}
	public static string unusableCharacters {
		get => (string) ProjectSettings.GetSetting("application/config/InvalidCharacters");
		set => UpdateSetting("config/InvalidCharacters", value);
	}
	public static int characterSet {
		get => (int) ProjectSettings.GetSetting("application/config/CharacterSet");
		set => UpdateSetting("config/CharacterSet", value);
	}
	public static string master {

		get => ((string) ProjectSettings.GetSetting("application/secret/Master"));
		set => SetMaster(value);
	}
	public static bool saveMaster {

		get => (bool) ProjectSettings.GetSetting("application/config/SaveMaster");
		set => SetSaveMaster(value);
	}
	public static string token {

		get => (string) ProjectSettings.GetSetting("application/secret/Token");
		set => UpdateSetting("secret/Token",value);
	}
	public static bool legacy {

		get => (bool) ProjectSettings.GetSetting("application/config/LegacyAlgorithm");
		set => UpdateSetting("config/LegacyAlgorithm", value);
	}
	public static bool safeMode {

		get;
		set;
	}
	public static string GetCharacters() {

		Debugger.Print("Getting Character List");

		string usuableCharacters = "";
		string currentCharacterSet = "";

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
		foreach(char i in currentCharacterSet) {

			if(unusableCharacters.Contains(i.ToString()) == false) {

				usuableCharacters += i;
			}
		}

		return usuableCharacters;
	}

	public static void SetCharacterCount(string value) {

		Debugger.Print("Updating Character Count");
		if(value.IsValidInteger()) {

			characterCount = value.ToInt();
		}
	}
	private static void SetMaster(string value) {

		Debugger.Print("Updating Master");
		UpdateSetting("secret/Master", value);
	}
	private static void SetSaveMaster(bool value) {

		UpdateSetting("config/SaveMaster", value);
	}
	private static void UpdateSetting(string settingName, System.Object value) {

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

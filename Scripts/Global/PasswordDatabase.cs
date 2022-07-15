using Godot;
using Godot.Collections;
using SavedPassword;
using NewConsole;

class PasswordDatabase : Node {

	[Signal] public delegate void PasswordCreated(PasswordData savedPassword);
	[Signal] public delegate void PasswordRemoved(PasswordData savedPassword);

	private const string fileName = "User_Passwords.sav";

	public Array<PasswordData> passwordList = new Array<PasswordData>();

	Dictionary groupList {
		get => new Dictionary();
	}

	public void Init() {

		Debugger.Print("Attempting To Get Save Data");
		try {

			foreach(PasswordData password in FileAccessSystem.ReadUserData(fileName)) {

				passwordList.Insert(FindIndexForPassword(password), password);
			};
			Debugger.Print("Save Data Found");
		}
		catch(FileNotFoundException e) {

			Debugger.Print(e.Message, Debugger.DebuggerState.STATE_WARNING);
			Debugger.Print("Creating New File");
			SaveToFile();
		}
	}

	/// <summary>
	/// Generate a new password and copy it to clipboard
	/// </summary>
	/// <param name="savedPassword"></param>
	public static void GeneratePassword(PasswordData savedPassword) {

		string password = "";

		string characters = GetCharacters(savedPassword);
		Debugger.Print(savedPassword.CharCount, Debugger.DebuggerState.STATE_WARNING);

		string seed = savedPassword.Legacy ?
			Settings.master + savedPassword.Key :
			Settings.token + Settings.master + savedPassword.Key;

		Debugger.Print("Creating New Password");
		
		GD.Seed(seed.Hash());
		for(int i=0; i<savedPassword.CharCount; i++) {

			password += characters[(int) Mathf.Round((int) GD.RandRange(0, characters.Length-1))];
		}

		OS.Clipboard = password;
		Debugger.Print("Password Copied To Clipboard");
	}

	/// <summary>
	/// Create a new saved password
	/// </summary>
	/// <param name="savedPassword"></param>
	public void CreatePassword(PasswordData savedPassword) {

		Debugger.Print("Checking If Password Group Exists");
		if(!groupList.Contains(savedPassword.Group)) {

			Debugger.Print("Password '" + savedPassword.Id + "' Group Non Existent");
		}
		AddPasswordToSaveData(savedPassword);
	}

	/// <summary>
	/// Removea  currently saved password
	/// </summary>
	/// <param name="savedPassword"></param>
	public void RemovePassword(PasswordData savedPassword) {

		Debugger.Print("Checking If Password: '" + savedPassword.Id+"' Exists");

		if(passwordList.Contains(savedPassword)) {

			Debugger.Print("Removing Password");
			passwordList.Remove(savedPassword);
		}
		SaveToFile();
		EmitSignal(nameof(PasswordRemoved), savedPassword);
	}

	/// <summary>
	/// Remove currently saved password
	/// </summary>
	/// <param name="passwordID"></param>
	public void RemovePassword(int passwordId) {

		Debugger.Print("Checking If Password: '" + passwordId + "' Exists");

		int passwordIndex = GetIndexOfPassword(passwordId);
		if(passwordIndex != -1) {

			Debugger.Print("Removing Password");
			PasswordData savedPassword = passwordList[passwordIndex];
			passwordList.Remove(savedPassword);
			EmitSignal(nameof(PasswordRemoved), savedPassword);
		}
	}

	/// <summary>
	/// Adds password to list of passwords
	/// </summary>
	/// <param name="savedPassword"></param>
	private void AddPasswordToSaveData(PasswordData savedPassword) {

		Debugger.Print("Adding Password: " + savedPassword.Id + " To Save Data");

		if(!passwordList.Contains(savedPassword)) {

			passwordList.Insert(FindIndexForPassword(savedPassword), savedPassword);
		}

		try {

			FileAccessSystem.WritePassword(fileName, savedPassword);
		}
		catch(FileNotFoundException e) {

			Debugger.Print(e.Message);
			SaveToFile();
		}

		EmitSignal(nameof(PasswordCreated), savedPassword);
	}

	/// <summary>
	/// Update a pre-existing password
	/// </summary>
	/// <param name="oldPasswordID"></param>
	/// <param name="newPassword"></param>
	public void UpdatePassword(int oldPasswordId, PasswordData newPassword) {

		Debugger.Print("Updating Password: "+ oldPasswordId);

		RemovePassword(oldPasswordId);

		passwordList.Insert(FindIndexForPassword(newPassword), newPassword);
		SaveToFile();

		EmitSignal(nameof(PasswordCreated), newPassword);
	}

	/// <summary>
	/// Returns list of group names
	/// </summary>
	/// <returns></returns>
	public Array GetGroupNames() {

		Debugger.Print("Retrieving List Of All Groups");
		return GetTree().GetNodesInGroup("Group");
	}
	public Array<PasswordData> GetPasswordsInGroup(string groupName) {

		Array<PasswordData> passwords = new Array<PasswordData>();
		foreach(PasswordData i in passwordList) {

			if(i.Group == groupName) {
			
				passwords.Add(i);
			}
		}
		return passwords;
	}
	public int GetPasswordIndexInGroup(int passwordId) {

		string targetGroup = "";
		Array<PasswordData> array = new Array<PasswordData>();

		foreach(PasswordData i in passwordList) {

			if(i.Id == passwordId) {
			
				targetGroup = i.Group;
			}
		}
		foreach(PasswordData i in passwordList) {

			if(i.Group == targetGroup) {

				array.Add(i);
			}
		}
		for(int i = 0; i < array.Count; i++) {

			if(array[i].Id == passwordId) {

				return i;
			}
		}
		return -1;
	}


	/// <summary>
	/// Saves the list of passwords to file
	/// </summary>
	public void SaveToFile() {

		Debugger.Print("Saving Passwords");

		Array<PasswordData> array = new Array<PasswordData>();

		foreach(PasswordData i in passwordList) {

			array.Add(i);
		}

		FileAccessSystem.WriteAllUserData(fileName, array);
	}

	/// <summary>
	/// Returns a string of useable characters for generating passwords
	/// </summary>
	/// <param name="savedPassword"></param>
	/// <returns></returns>
	private static string GetCharacters(PasswordData savedPassword) {

		Debugger.Print("Getting Character List");

		string usuableCharacters = "";
		string currentCharacterSet = "";

		Debugger.Print("Accounting For Character Set");

		switch(savedPassword.CharSet) {

			case CharacterSets.SET_ALL:
				currentCharacterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ`1234567890-=[]\\;',./~!@#$%^&*()_+{}|:<>?";
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

			if(savedPassword.CharInvalid.Contains(i.ToString()) == false) {

				usuableCharacters += i;
			}
		}

		return usuableCharacters;
	}

	private int FindIndexForPassword(PasswordData newPassword, int minIndex = 0, int maxIndex = -1) {

		if(maxIndex == -1) {

			maxIndex = passwordList.Count;
		}
		if(maxIndex == minIndex) {

			return minIndex;
		}
		else {

			int currentIndex = Mathf.FloorToInt((minIndex + maxIndex) / 2);

			// Password index Greater then current, Moves password up in list
			if(newPassword.Index > passwordList[currentIndex].Index) {

				return FindIndexForPassword(newPassword, minIndex, currentIndex);
			}
			// Password index Less then current, Moves password down in list
			if(newPassword.Index < passwordList[currentIndex].Index) {

				return FindIndexForPassword(newPassword, currentIndex + 1, maxIndex);
			}
			// Password index Equals current, Move password down in list
			if(newPassword.Index == passwordList[currentIndex].Index) {

				return FindIndexForPassword(newPassword, currentIndex + 1, maxIndex);
			}
		}
		return passwordList.Count - 1;
	}
	private int GetIndexOfPassword(PasswordData targetPassword) {

		for(int i = 0; i < passwordList.Count; i++) {

			if(targetPassword.Id == passwordList[i].Id) {

				return i;
			}
		}
		return -1;
	}
	private int GetIndexOfPassword(int passwordId) {

		for(int i = 0; i < passwordList.Count; i++) {

			if(passwordId == passwordList[i].Id) {

				return i;
			}
		}
		return -1;
	}
}
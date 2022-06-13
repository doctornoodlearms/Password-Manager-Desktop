using Godot;
using System;
using System.Collections.Generic;
using SavedPassword;
using NewConsole;

class PasswordDatabase : Node {

	[Signal] public delegate void PasswordCreated(PasswordData savedPassword);
	[Signal] public delegate void PasswordRemoved(PasswordData savedPassword);

	private const String fileName = "User_Passwords.sav";

	public Dictionary<String, PasswordData> passwordList = new Dictionary<String, PasswordData>();
	

	Godot.Collections.Dictionary groupList {
		get => new Godot.Collections.Dictionary();
	}

	public void Init() {

		Debugger.Print("Attempting To Get Save Data");
		try {

			foreach(PasswordData password in FileAccessSystem.ReadUserData(fileName)) {

				passwordList.Add(password.GetHashCode().ToString(), password);
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
	public void GeneratePassword(PasswordData savedPassword) {

		String password = "";

		String characters = GetCharacters(savedPassword);
		Debugger.Print(savedPassword.CharCount, Debugger.DebuggerState.STATE_WARNING);

		String seed = savedPassword.Legacy ?
			Settings.master + savedPassword.Key :
			Settings.token + Settings.master + savedPassword.Key;

		Debugger.Print("Creating New Password");
		
		GD.Seed(seed.Hash());
		for(Int32 i=0; i<savedPassword.CharCount; i++) {

			password += characters[(Int32)Mathf.Round((Single)GD.RandRange(0, characters.Length-1))];
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

		if(passwordList.ContainsKey(savedPassword.Id)) {

			Debugger.Print("Removing Password");
			passwordList.Remove(savedPassword.Id);
		}
		SaveToFile();
		EmitSignal(nameof(PasswordRemoved), savedPassword);
	}

	/// <summary>
	/// Remove currently saved password
	/// </summary>
	/// <param name="passwordID"></param>
	public void RemovePassword(String passwordID) {

		Debugger.Print("Checking If Password: '" + passwordID + "' Exists");
		if(passwordList.ContainsKey(passwordID)) {

			Debugger.Print("Removing Password");
			PasswordData savedPassword = passwordList[passwordID];
			passwordList.Remove(passwordID);
			EmitSignal(nameof(PasswordRemoved), savedPassword);
		}
	}

	/// <summary>
	/// Adds password to list of passwords
	/// </summary>
	/// <param name="savedPassword"></param>
	private void AddPasswordToSaveData(PasswordData savedPassword) {

		Debugger.Print("Adding Password: " + savedPassword.Id + " To Save Data");

		if(!passwordList.ContainsKey(savedPassword.Id)) {

			passwordList.Add(savedPassword.Id, savedPassword);
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
	public void UpdatePassword(String oldPasswordID, PasswordData newPassword) {

		Debugger.Print("Updating Password: "+ oldPasswordID);

		RemovePassword(oldPasswordID);

		passwordList.Add(newPassword.Id, newPassword);
		SaveToFile();

		EmitSignal(nameof(PasswordCreated), newPassword);
	}

	/// <summary>
	/// Returns list of group names
	/// </summary>
	/// <returns></returns>
	public Godot.Collections.Array GetGroupNames() {

		Debugger.Print("Retrieving List Of All Groups");
		return GetTree().GetNodesInGroup("Group");
	}

	/// <summary>
	/// Saves the list of passwords to file
	/// </summary>
	public void SaveToFile() {

		Debugger.Print("Saving Passwords");

		Godot.Collections.Array<PasswordData> array = new Godot.Collections.Array<PasswordData>();

		if(passwordList.Count > 0) {

			IEnumerator<PasswordData> enumerator = passwordList.Values.GetEnumerator();
			while(enumerator.MoveNext()) {

				array.Add(enumerator.Current);
			}
		}
		FileAccessSystem.WriteAllUserData(fileName, array);
	}

	/// <summary>
	/// Returns a string of useable characters for generating passwords
	/// </summary>
	/// <param name="savedPassword"></param>
	/// <returns></returns>
	private String GetCharacters(PasswordData savedPassword) {

		Debugger.Print("Getting Character List");

		String usuableCharacters = "";
		String currentCharacterSet = "";

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
		foreach(Char i in currentCharacterSet) {

			if(savedPassword.CharInvalid.Contains(i.ToString()) == false) {

				usuableCharacters += i;
			}
		}

		return usuableCharacters;
	}
}
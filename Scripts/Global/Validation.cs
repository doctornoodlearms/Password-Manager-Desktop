using System;
using Godot;
using NewConsole;
public class Validation : Node {

	const String fileName = "PasswordGenToken.sav";
	const String aesKey = "6E5A7234753778214125442A472D4A61";

	[Signal]
	public delegate void UserPrompted();

	public static String GetUID() {
	
		byte[] data = new byte[64];
		System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();
		rng.GetBytes(data);
		return BitConverter.ToString(data).Replace("-", "").ToLower();
	}

	//	
	//
	// -----------TOKEN LOGIC------------
	//			Local		User		Result
	//			
	//			TRUE		TRUE		Normal
	//			TRUE		FALSE		File on flash-drive: Clear master
	//			FALSE		TRUE		Local token deleted or unrecognized program
	//			FALSE		FALSE		Program inactive prompt user to activate connecting to my pc to obtain a token

	public void Validate() {

		// Read user token from file
		String userToken = "";

		if(!Settings.saveMaster) {

			PromptUserMaster();
			return;
		}

		try {

			Debugger.Print("Retrieving User Token");
			userToken = FileAccessSystem.ReadStringFromUser(fileName);
		}
		catch(FileNotFoundException) {

			Debugger.Print("Failed To Find File", Debugger.DebuggerState.STATE_ERROR);
		}

		// Validation Algorithm

		// Program hasn't been activated (neither token found)
		if(userToken == "" && Settings.token == "") {

			Debugger.Print("No token created", Debugger.DebuggerState.STATE_WARNING);
			Debugger.Print("Creating new token");

			String token = GetUID();
			Settings.token = token;
			FileAccessSystem.WriteStringToUser(fileName, EncryptString(token));
			return;
		}

		// Normal (Both tokens found)
		if(userToken == Settings.token) {

			Debugger.Print("Tokens Match");
			Settings.token = DecryptString(Settings.token);
			if(Settings.master != "") {

				Settings.master = Validation.DecryptString(Settings.master.PadRight(32)).Replace(" ","");
			}
			return;
		}

		// Running on flashdrive (user token missing and local token found)
		if(userToken == "" && Settings.token != "") {

			Debugger.Print("Running as flashdrive", Debugger.DebuggerState.STATE_WARNING);
			PromptUserMaster();
			return;
		}

		// Idk what this means (user token found, and local token missing)
		if(userToken != "" && Settings.token == "") {

			Debugger.Print("Local token missing, user token found", Debugger.DebuggerState.STATE_WARNING);
			PromptUserMaster();
			return;
		}

		// Tokens don't match but aren't missing
		if(userToken != Settings.token && (userToken != "" || Settings.token != "")) {

			Debugger.Print("Token Mismatch", Debugger.DebuggerState.STATE_WARNING);
			PromptUserMaster();
			return;
		}
	}

	public static String EncryptString(String data) {

		Debugger.Print("Encrypting String");

		AESContext aes = new AESContext();
		aes.Start(AESContext.Mode.EcbEncrypt, aesKey.ToUTF8());
		Byte[] encrypted = aes.Update(data.ToUTF8());

		String encryptedString = "";
		foreach(Byte @byte in encrypted) {

			encryptedString += @byte.ToString().ToInt().ToString("x2");
		}
		return encryptedString;
	}

	public static String DecryptString(String secret) {

		Debugger.Print("Decrypting String");

		Byte[] decrypt = new Byte[secret.Length / 2];

		for(int i = 0; i <= secret.Length - 2; i += 2) {
			decrypt[i / 2] = (Byte) (secret[i].ToString() + secret[i + 1].ToString()).HexToInt();
		}
		
		AESContext aes = new AESContext();
		aes.Start(AESContext.Mode.EcbDecrypt, aesKey.ToUTF8());
		Byte[] decryptedString = aes.Update(decrypt);
		aes.Finish();

		String returnValue = "";
		foreach(Byte @byte in decryptedString) {

			returnValue += (char) @byte;
		}
		return returnValue;
	}

	// Confirms the master password entered by the user while in flashdrive mode
	public static Boolean ConfirmMaster(String enteredMaster) {

		if(EncryptString(enteredMaster.PadRight(32)) == Settings.master) {

			Settings.token = DecryptString(Settings.token);
			Settings.master = DecryptString(Settings.master.PadRight(32)).Replace(" ", "");

			return true;
		}
		else {

			return false;
		}
	}

	// PRompt the user to enter the master password
	private void PromptUserMaster() {

		GetTree().Root.CallDeferred("add_child", GD.Load<PackedScene>("res://Scenes/Instances/Popup_MasterPrompt.tscn").Instance());
	}

	public static void CopyUserToken() {

		Settings.token = DecryptString(FileAccessSystem.ReadStringFromUser(fileName));
	}
}
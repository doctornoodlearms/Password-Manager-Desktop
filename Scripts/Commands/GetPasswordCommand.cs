using Godot;
using NewConsole;
using CommandHandler;
using SavedPassword;

public class GetPasswordCommand : BaseCommand {

	public override string Name => "GetPasswordCommand";
	public override string[] AliasList => new string[] { "pass", "-pass" };
	
	// Syntax: -pass -master [Master] -key [Key]
	public override void Execute(string[] args) {

		Settings.safeMode = true;

		if(args[1] == "-master" && args[3] == "-key") {

			Settings.master = args[2];
			PasswordDatabase.GeneratePassword(new PasswordData(args[4]));
		}
	}
}
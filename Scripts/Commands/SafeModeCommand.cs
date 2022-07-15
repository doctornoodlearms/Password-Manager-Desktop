using Godot;
using NewConsole;
using CommandHandler;

public class SafeModeCommand : BaseCommand {

	public override string Name => "SafeModeCommand";
	public override string[] AliasList => new string[] { "safe", "-safe" };
	public override void Execute(string[] args) {
		
		Settings.safeMode = true;
	}
}
using CommandHandler;

public class CopyUserCommand : BaseCommand {

	public override string Name => "updatetoken";
	public override string[] AliasList => new string[] { "update", "token", "-update", "-token"};
	public override void Execute(string[] args) {

		Validation.CopyUserToken();
	}
}


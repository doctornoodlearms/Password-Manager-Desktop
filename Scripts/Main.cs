using Godot;
using SavedPassword;
using NewConsole;

public class Main : VBoxContainer
{

	public override void _Ready() {

		string version = (string)ProjectSettings.GetSetting("application/config/version");
		GetNode<Label>("Version").Text = "Version: " + version;

		GetNode<Validation>("/root/Validation").Connect(nameof(Validation.UserPrompted), this, nameof(GD.Print), new Godot.Collections.Array { "User Prompted" });

		GetNode<LineEdit>("Vbox/Master").Connect("text_entered", this, nameof(OnMasterTextEntered));
		GetNode<LineEdit>("Vbox/Key").Connect("text_entered", this, nameof(OnKeyTextEntered));

		GetNode<LineEdit>("Options/CountMargin/Count").Connect("text_entered", this, nameof(OnCountTextEntered));
		GetNode<LineEdit>("Options/UnusableMargin/Unusable").Connect("text_entered", this, nameof(OnUnusableTextEntered));

		GetNode<Button>("ChangeScene/SettingsMargin/Settings").Connect("pressed", this, nameof(OnSettingsPressed));
		GetNode<Button>("ChangeScene/PasswordMargin/Passwords").Connect("pressed", this, nameof(OnPasswordsPressed));

		GetNode<ConfirmationDialog>("../../ConfirmationDialog").Connect("confirmed", this, nameof(ChangeMaster));
		GetNode<ConfirmationDialog>("../../ConfirmationDialog").GetCloseButton().Connect("pressed", this, nameof(ClearMasterText));
		GetNode<ConfirmationDialog>("../../ConfirmationDialog").GetCancel().Connect("pressed", this, nameof(ClearMasterText));

		GetNode<LineEdit>("Vbox/Master").Secret = Settings.hideMaster;

		if(Settings.master == "") {

			GetNode<LineEdit>("Vbox/Master").GrabFocus();
		}
		else {

			GetNode<LineEdit>("Vbox/Key").GrabFocus();
		}
	}

	private void OnMasterTextEntered(string value) {

		GetNode<ConfirmationDialog>("../../ConfirmationDialog").PopupCentered();
	}
	private void ClearMasterText() {

		Debugger.Print("Clearing Master Text");
		GetNode<LineEdit>("Vbox/Master").Text = "";
	}

	private void OnKeyTextEntered(string value) {

		GetNode<LineEdit>("Vbox/Key").Text = "";
		PasswordDatabase.GeneratePassword(new PasswordData(value, newLegacy: Settings.legacy, newCount: Settings.characterCount, newSet: (CharacterSets) Settings.characterSet, newInvalid: Settings.unusableCharacters));
	}
	private void OnCountTextEntered(string value) {

		GetNode<LineEdit>("Options/CountMargin/Count").Text = "";
		if(value.IsValidInteger()) {

			Settings.SetCharacterCount(value);
		}
	}
	private void OnUnusableTextEntered(string value) {

		GetNode<LineEdit>("Options/UnusableMargin/Unusable").Text = "";
		Settings.unusableCharacters = value;
	}

	private void OnSettingsPressed() {

		GetTree().ChangeScene("res://Scenes/Settings.tscn");
	}
	private void OnPasswordsPressed() {

		GetTree().ChangeScene("res://Scenes/Passwords.tscn");
	}

	private void ChangeMaster() {

		Debugger.Print("Changing Master");
		Settings.master = GetNode<LineEdit>("Vbox/Master").Text;
		ClearMasterText();
	}
}
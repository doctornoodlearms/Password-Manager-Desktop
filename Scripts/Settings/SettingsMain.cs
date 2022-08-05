using Godot;
using NewConsole;
public class SettingsMain : VBoxContainer {

	private LineEdit characterCount;
	private LineEdit unusableCharacters;
	private OptionButton characterSet;
	private CheckButton hideMaster;
	private CheckButton saveMaster;

	public override void _Ready() {

		CheckButton legacy = GetNode<CheckButton>("Legacy/Value");

		characterCount = GetNode<LineEdit>("CharacterCount/Value");
		unusableCharacters = GetNode<LineEdit>("UnusableCharacters/Value");
		characterSet = GetNode<OptionButton>("CharacterSet/Value");
		hideMaster = GetNode<CheckButton>("HideMaster/Value");
		saveMaster = GetNode<CheckButton>("SaveMaster/Value");
		

		characterCount.PlaceholderText = Settings.characterCount.ToString();
		characterSet.Selected = (int) Settings.characterSet;
		unusableCharacters.PlaceholderText = Settings.unusableCharacters;

		hideMaster.Pressed = Settings.hideMaster;
		hideMaster.Text = hideMaster.Pressed ? "Enabled" : "Disabled";

		saveMaster.Pressed = Settings.saveMaster;
		saveMaster.Text = saveMaster.Pressed ? "Enabled" : "Disabled";

		legacy.Pressed = Settings.legacy;


		characterCount.Connect("text_entered", this, nameof(OnCharacterCountTextEntered));
		unusableCharacters.Connect("text_entered", this, nameof(OnUnusableTextEntered));
		characterSet.Connect("item_selected", this, nameof(OnCharacterSetItemSelected));
		hideMaster.Connect("toggled", this, nameof(OnMasterToggled));
		saveMaster.Connect("toggled",this, nameof(OnSaveMasterToggled));
		legacy.Connect("toggled", this , nameof(OnLegacyPressed));

		GetNode<Button>("/root/Control/Background/MarginContainer/Back").Connect("pressed", this, nameof(OnBackPressed));
		;
	}

	public override void _Input(InputEvent @event) {

		if(@event.IsActionPressed("ui_cancel")) {

			GetTree().ChangeScene("res://Scenes/Main.tscn");
		}
	}

	private void OnCharacterCountTextEntered(string value) {

		characterCount.Text = "";
		characterCount.PlaceholderText = value;
		Settings.SetCharacterCount(value);
	}
	private void OnUnusableTextEntered(string value) {

		unusableCharacters.Text = "";
		unusableCharacters.PlaceholderText = value;
		Settings.unusableCharacters = value;
	}
	private void OnCharacterSetItemSelected(int value) {

		Settings.characterSet = value;
	}
	private void OnMasterToggled(bool value) {

		hideMaster.Text = value ? "Enabled" : "Disabled";
		Settings.hideMaster = value;
	}
	private void OnSaveMasterToggled(bool value) {

		saveMaster.Text = value ? "Enabled" : "Disabled";
		Settings.saveMaster = value;
	}
	private void OnLegacyPressed(bool value) {

		Settings.legacy = value;
	}

	private void OnBackPressed() {

		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}
}
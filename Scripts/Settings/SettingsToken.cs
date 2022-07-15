using Godot;

public class SettingsToken : VBoxContainer {

	public override void _Ready() {

		
	}

	public void On_CopyUser_Pressed() {

		Validation.CopyUserToken();		
	}
}
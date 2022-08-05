using Godot;
using NewConsole;

public class BottomPanel : MarginContainer
{

	public override void _Ready() {

		GetNode<Button>("NewPassword").Connect("pressed",this,nameof(OnNewPasswordPressed));
		GetNode<Button>("Back").Connect("pressed", this, nameof(OnBackPressed));
	}

	private void OnNewPasswordPressed() {

		GetNode<PopupHandler>("/root/Control/Popups").PopupPasswordOptions();
	}
	private void OnBackPressed() {

		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}
}
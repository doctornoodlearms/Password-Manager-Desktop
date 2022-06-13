using System;
using Godot;
using NewConsole;

public class FlashdrivePopup : VBoxContainer {

	public override void _Ready() {

		GetNode<Button>("Accept").Connect("pressed",this, nameof(onAcceptPressed));
		GetNode<CheckButton>("Hbox/Show").Connect("toggled", this, nameof(onShowPressed));

		GetNode<Control>("Hbox/LineEdit").GrabFocus();
	}

	private void onAcceptPressed() {

		if(Validation.ConfirmMaster(GetNode<LineEdit>("Hbox/LineEdit").Text)) {

			GetNode<Control>("/root/Control/PanelContainer/Vbox/Vbox/Key").GrabFocus();
			GetNode("/root/FlashdrivePopup").QueueFree();
		}
		else {

			GetNode<LineEdit>("Hbox/LineEdit").Text = "";
		}
	}

	private void onShowPressed(Boolean value) {

		GetNode<LineEdit>("Hbox/LineEdit").Secret = !value;
	}
}
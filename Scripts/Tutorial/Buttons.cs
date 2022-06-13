using Godot;
using System;
using NewConsole;
public class Buttons : HBoxContainer
{

	TabContainer tab;

	public override void _Ready() {

		GetNode<Button>("Back").Connect("pressed", this, nameof(OnBackPressed));
		GetNode<Button>("Continue").Connect("pressed", this, nameof(OnContinuePressed));

		tab = GetNode<TabContainer>("../Margin/Tabs");
	}

	private void OnContinuePressed() {

		if(tab.CurrentTab >= tab.GetTabCount()-1) {

			GetTree().ChangeScene("res://Scenes/Settings.tscn");
		}
		else {

			tab.CurrentTab += 1;
		}
	}

	private void OnBackPressed() {

		if(tab.CurrentTab <= 0) {

			GetTree().ChangeScene("res://Scenes/Settings.tscn");
		}
		else {

			tab.CurrentTab -= 1;
		}
	}
}

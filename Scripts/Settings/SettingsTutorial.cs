using Godot;

public class SettingsTutorial : Button
{

	public override void _Pressed() {

		GetTree().ChangeScene("res://Scenes/Explanation.tscn");
	}
}

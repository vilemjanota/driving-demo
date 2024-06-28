using Godot;
using System;

public partial class Menu : Control
{
	private void _on_start_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/level_0.tscn");
	}


	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}

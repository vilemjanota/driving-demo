using Godot;
using System;

public partial class UI : Control
{
	private float time = 0;
	private bool finish = false;
	
	public override void _PhysicsProcess(double delta)
	{		
		if(!finish)
		{
			time += (float)delta;
			GetNode<Label>("Timer").Text = time.ToString("0.00");			
		}

	}
	
	private void _on_finish_player_touched_finish()
	{
		GetNode<Label>("Win").Text = "You win!";
		finish = true;
	}
	private void _on_quit_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}
}




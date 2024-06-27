using Godot;
using System;

public partial class UI : Control
{
	private void _on_finish_player_touched_finish()
	{
		GetNode<Label>("Label").Text = "You win!";
	}
}

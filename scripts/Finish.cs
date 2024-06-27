using Godot;
using System;

public partial class Finish : Area3D
{
	// Define a signal named 'player_touched_finish'
	[Signal]
	public delegate void PlayerTouchedFinishEventHandler();

	private void _on_body_entered(Node3D body)
	{
		if (body is Player)
		{
			// Emit the 'player_touched_finish' signal
			EmitSignal(nameof(PlayerTouchedFinish));
		}
	}
}

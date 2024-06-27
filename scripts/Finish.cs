using Godot;
using System;

public partial class Finish : Area3D
{
	private void _on_body_entered(Node3D body)
	{
		if (body is Player) // Assuming 'Player' is the class name of your player character
		{
			GD.Print("Player has touched the finish object!");
		}
	}
	
}




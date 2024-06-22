using Godot;

public partial class Player : CharacterBody3D
{
	// How fast the player moves in meters per second.
	[Export]
	public int Speed { get; set; } = 0;
	// The downward acceleration when in the air, in meters per second squared.
	[Export]
	public int FallAcceleration { get; set; } = 75;

	private bool movingForward = true;
	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		
		var direction = Vector3.Zero;
		if(movingForward)
		{
			direction.Z = -1;
		}
		else
		{
			direction.Z = 1;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			movingForward = true;
			Speed = 25;
		}
		if (Input.IsActionPressed("move_back"))
		{
			movingForward = false;
			Speed = 15;
		}
		if (Input.IsActionPressed("move_right"))
		{
			GetNode<Node3D>("Pivot").RotateY(Speed * 0.1f * -0.017f);
		}
		if (Input.IsActionPressed("move_left"))
		{
			GetNode<Node3D>("Pivot").RotateY(Speed * 0.1f * 0.017f);
		}
		if(Speed>0)
		{
			Speed -= 1;
		}
		GD.Print(Speed);
		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GD.Print(GetNode<Node3D>("Pivot").Rotation);
			direction = direction.Rotated(new Vector3(0, 1, 0),GetNode<Node3D>("Pivot").Rotation.Y);
		}

		// Ground velocity
		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;
		// Vertical velocity
		if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}

		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
}

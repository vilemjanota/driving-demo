using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int MaxSpeed { get; set; } = 30;
	[Export]
	public int Acceleration { get; set; } = 10;
	
	private float Speed;
	private bool MovingForward = true;
	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;
		direction.Z = -1;
		if (Input.IsActionPressed("move_forward"))
		{
			MovingForward = true;
			if(Speed < MaxSpeed)
			{
				Speed += Acceleration;
			}
			else
			{
				Speed = MaxSpeed;
			}
		}
		if (Input.IsActionPressed("move_back"))
		{
			MovingForward = false;
			if(Speed > -MaxSpeed/2)
			{
				Speed -= Acceleration/2;
			}
			else
			{
				Speed = -MaxSpeed/2;
			}
		}
		if (Input.IsActionPressed("move_right"))
		{
			GetNode<Node3D>("Pivot").RotateY(Speed * 0.05f * -0.017f);
		}
		if (Input.IsActionPressed("move_left"))
		{
			GetNode<Node3D>("Pivot").RotateY(Speed * 0.05f * 0.017f);
		}
		if (Input.IsActionPressed("brake"))
		{
			if(Speed>0)
			{
				Speed -= 1;
			}
			else if(Speed<0)
			{
				Speed += 1;
			}
		}
		if(Speed>0)
		{
			Speed -= 0.1f;
		}
		else if(Speed<0)
		{
			Speed += 0.1f;
		}
		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
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

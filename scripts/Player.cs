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
	private Node3D CameraPivot;
	private Node3D Pivot;
	public override void _Ready()
	{
		Pivot = GetNode<Node3D>("Pivot");
		CameraPivot = GetNode<Node3D>("CameraPivot");
	}
	
	public void UpdateCamera(double delta)
	{
		//Follow the player
		CameraPivot.GlobalPosition = CameraPivot.GlobalPosition.Lerp(Pivot.GlobalPosition,(float)delta * 6.0f);
		//Delayed rotation
		Vector3 rotation = CameraPivot.Rotation;
		rotation.X = Mathf.LerpAngle(rotation.X, Pivot.Rotation.X, 2.0f * (float)delta);
		rotation.Y = Mathf.LerpAngle(rotation.Y, Pivot.Rotation.Y, 2.0f * (float)delta);
		rotation.Z = Mathf.LerpAngle(rotation.Z, Pivot.Rotation.Z, 2.0f * (float)delta);
		CameraPivot.Rotation = rotation;
	}

	public override void _PhysicsProcess(double delta)
	{		
		UpdateCamera(delta);
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

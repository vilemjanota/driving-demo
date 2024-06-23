using Godot;

public partial class Player : CharacterBody3D
{
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int MaxSpeed { get; set; } = 30;
	[Export]
	public int Acceleration { get; set; } = 10;
	[Export]
	public float TurnRotation { get; set; } = 0.05f * 0.017f;
	[Export]
	public float DriftRotation { get; set; } = 0.2f * 0.017f;

	private float Speed;
	private Vector3 _targetVelocity = Vector3.Zero;
	private Node3D CameraPivot;
	private Node3D Pivot;
	private Node3D Model;
	private bool Drift = false;
	
	public override void _Ready()
	{
		Pivot = GetNode<Node3D>("Pivot");
		CameraPivot = GetNode<Node3D>("CameraPivot");
		Model = Pivot.GetNode<Node3D>("Model");
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

	public void UpdateSpeed()
	{
		if(Input.IsActionPressed("move_forward"))
		{
			if(Speed < MaxSpeed)
			{
				Speed += Acceleration;
			}
			else
			{
				Speed = MaxSpeed;
			}
		}
		if(Input.IsActionPressed("move_back"))
		{
			if(Speed > -MaxSpeed/2)
			{
				Speed -= Acceleration/2;
			}
			else
			{
				Speed = -MaxSpeed/2;
			}
		}
		if(Input.IsActionPressed("brake"))
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
		if(Drift)
		{
			Speed -= 3;
		}
	}

	public Vector3 UpdateDirection()
	{
		var Direction = new Vector3(0,0,-1);
		if(Input.IsActionPressed("move_right"))
		{
			Pivot.RotateY(Speed * -TurnRotation);
			// Enable drift and apply drift rotation when moving right and braking
			if (Input.IsActionPressed("brake") && !Drift)
			{
				Drift = true;
				Model.RotateY(Speed * -DriftRotation);
			}
		}
		if(Input.IsActionPressed("move_left"))
		{
			Pivot.RotateY(Speed * TurnRotation);
			// Enable drift and apply drift rotation when moving left and braking
			if(Input.IsActionPressed("brake") && !Drift)
			{
				Drift = true;
				Model.RotateY(Speed * DriftRotation);
			}
		}
		// Reset drift state and model rotation when brake is released
		if(!Input.IsActionPressed("brake") && Drift)
		{
			Drift = false;
			Model.Rotation = new Vector3(0,0,0);
		}
		if(Direction != Vector3.Zero)
		{
			// Normalize and rotate direction vector based on pivot rotation
			Direction = Direction.Normalized();
			Direction = Direction.Rotated(new Vector3(0, 1, 0),Pivot.Rotation.Y);
		}
		return Direction;
	}

	public override void _PhysicsProcess(double delta)
	{		
		UpdateCamera(delta);
		UpdateSpeed();
		var Direction = UpdateDirection();

		// Ground velocity
		_targetVelocity.X = Direction.X * Speed;
		_targetVelocity.Z = Direction.Z * Speed;
		// Apply gravity if player is not on the floor
		if(!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
		}
		// Moving the character
		Velocity = _targetVelocity;
		MoveAndSlide();
	}
}

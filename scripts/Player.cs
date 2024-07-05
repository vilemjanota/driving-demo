using Godot;
using DriftHandlerScript;

public partial class Player : CharacterBody3D
{
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int MaxSpeed { get; set; } = 30;
	[Export]
	public int Acceleration { get; set; } = 10;
	[Export]
	public float TurnRotation { get; set; } = 0.05f * ANGLE;

	private const float ANGLE  = 0.017f;
	private float speed;
	private Vector3 _targetVelocity = Vector3.Zero;
	private Node3D cameraPivot;
	private Node3D pivot;
	private Node3D model;
	private AnimationPlayer animationPlayer;
	private DriftHandler driftHandler;
	private bool Drift = false;
	
	public override void _Ready()
	{
		pivot = GetNode<Node3D>("Pivot");
		cameraPivot = GetNode<Node3D>("CameraPivot");
		model = pivot.GetNode<Node3D>("Model");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		driftHandler = new DriftHandler(animationPlayer);
	}
	
	public void UpdateCamera(double delta)
	{
		//Follow the player
		cameraPivot.GlobalPosition = cameraPivot.GlobalPosition.Lerp(pivot.GlobalPosition,(float)delta * 6.0f);
		//Delayed rotation
		Vector3 rotation = cameraPivot.Rotation;
		rotation.X = Mathf.LerpAngle(rotation.X, pivot.Rotation.X, 2.0f * (float)delta);
		rotation.Y = Mathf.LerpAngle(rotation.Y, pivot.Rotation.Y, 2.0f * (float)delta);
		rotation.Z = Mathf.LerpAngle(rotation.Z, pivot.Rotation.Z, 2.0f * (float)delta);
		cameraPivot.Rotation = rotation;
	}

	public void UpdateSpeed()
	{
		if(Input.IsActionPressed("move_forward"))
		{
			if(speed < MaxSpeed)
			{
				speed += Acceleration;
			}
			else
			{
				speed = MaxSpeed;
			}
		}
		if(Input.IsActionPressed("move_back"))
		{
			if(speed > -MaxSpeed/2)
			{
				speed -= Acceleration/2;
			}
			else
			{
				speed = -MaxSpeed/2;
			}
		}
		if(Input.IsActionPressed("brake"))
		{
			if(speed>0)
			{
				speed -= 1;
			}
			else if(speed<0)
			{
				speed += 1;
			}
		}
		if(speed>0)
		{
			speed -= 0.1f;
		}
		else if(speed<0)
		{
			speed += 0.1f;
		}
	}

	public Vector3 UpdateDirection()
	{
		var Direction = new Vector3(0,0,-1);
		if(Input.IsActionPressed("move_right"))
		{
			pivot.RotateY(speed * -TurnRotation);
		}
		if(Input.IsActionPressed("move_left"))
		{
			pivot.RotateY(speed * TurnRotation);
		}
		if(Direction != Vector3.Zero)
		{
			// Normalize and rotate direction vector based on pivot rotation
			Direction = Direction.Normalized();
			Direction = Direction.Rotated(new Vector3(0, 1, 0),pivot.Rotation.Y);
		}
		return Direction;
	}

	public override void _PhysicsProcess(double delta)
	{		
		UpdateCamera(delta);
		UpdateSpeed();
		var Direction = UpdateDirection();
		driftHandler.UpdateDrift();
		// Ground velocity
		_targetVelocity.X = Direction.X * speed;
		_targetVelocity.Z = Direction.Z * speed;
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

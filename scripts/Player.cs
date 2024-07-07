using Godot;
using DriftHandlerScript;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public int FallAcceleration { get; set; } = 75;
	[Export]
	public int MaxSpeed { get; set; } = 30;
	[Export]
	public int Acceleration { get; set; } = 10;
	[Export]
	public float TurnRotation { get; set; } = 0.05f;
	[Export]
	public AudioStreamPlayer AudioPlayer { get; set; }
	[Export]
	public Node3D CameraPivot;
	[Export]
	public Node3D Pivot;
	[Export]
	public AnimationPlayer AnimationPlayer;

	private Vector3 _targetVelocity = Vector3.Zero;
	private const float ANGLE  = 0.017f;
	private float speed;
	private DriftHandler driftHandler;
	private bool canMove = false;

	private bool Drift = false;
	
	public override void _Ready()
	{
		driftHandler = new DriftHandler(AnimationPlayer);
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

	private void OnCountdownFinished()
	{
		canMove = true; // Enable movement when the countdown is finished
	}

	public void UpdateSpeed()
	{
		if (!canMove) return; // Prevent movement if the countdown is not finished

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
		if(AnimationPlayer.CurrentAnimation == "DriftRight" || AnimationPlayer.CurrentAnimation == "DriftLeft")
		{
			speed -= 5;
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
			Pivot.RotateY(speed * -TurnRotation  * ANGLE);
		}
		if(Input.IsActionPressed("move_left"))
		{
			Pivot.RotateY(speed * TurnRotation * ANGLE);
		}
		if(Direction != Vector3.Zero)
		{
			// Normalize and rotate direction vector based on Pivot rotation
			Direction = Direction.Normalized();
			Direction = Direction.Rotated(new Vector3(0, 1, 0),Pivot.Rotation.Y);
		}
		return Direction;
	}

	void UpdateSound()
	{
		AudioPlayer.PitchScale = 0.03f * Math.Abs(speed);
		if(Math.Abs(speed) > 0.1 && !AudioPlayer.HasStreamPlayback())
		{
			AudioPlayer.Play();
		}
		else if(Math.Abs(speed) < 0.1 && AudioPlayer.HasStreamPlayback())
		{
			AudioPlayer.Stop();
		}
	}

	public override void _PhysicsProcess(double delta)
	{		
		UpdateCamera(delta);
		UpdateSpeed();
		UpdateSound();
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

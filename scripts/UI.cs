using Godot;
using System;
using System.Threading.Tasks;

public partial class UI : Control
{

	[Signal]
	public delegate void CountdownFinishedEventHandler();

	[Export]
	public Label TimerLabel { get; set; }
	[Export]
	public Label CountdownLabel { get; set; }
	[Export]
	public AudioStreamPlayer AudioPlayer { get; set; }

	private float time = 0;
	private bool playing = false;
	
	public override void _Ready()
	{
		Countdown();
	}
	
	public override void _PhysicsProcess(double delta)
	{		
		if(playing)
		{
			time += (float)delta;
			TimerLabel.Text = time.ToString("0.00");			
		}
	}
	
	private void _on_finish_player_touched_finish()
	{
		GetNode<Label>("Win").Text = "You win!";
		playing = false;
	}
	private void _on_quit_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}
	private async void Countdown()
	{
		CountdownLabel.Text = "3";
		AudioPlayer.Play();
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
		CountdownLabel.Text = "2";
		AudioPlayer.Play();
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
		CountdownLabel.Text = "1";
		AudioPlayer.Play();
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
		CountdownLabel.Visible = false;
		playing = true;
		EmitSignal(nameof(CountdownFinished));
	}
}




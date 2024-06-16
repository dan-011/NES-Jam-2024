using General;
using Godot;
using System;

public partial class PlayerUI : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		healthAnimation0 = GetNode<AnimatedSprite2D>("HealthAnimation0");
		healthAnimation1 = GetNode<AnimatedSprite2D>("HealthAnimation1");
		healthAnimation2 = GetNode<AnimatedSprite2D>("HealthAnimation2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		scoreLabel.Text = GameData.Instance.GetScore().ToString();
		float health = GameData.Instance.GetHealth();
		DisplayHealth(health);
	}

	private void DisplayHealth(float health) {
		if(health == 100f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "full";
		}
		else if(health > 82.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "two-thirds";
		}
		else if(health > 75f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "one-third";
		}
		else if(health > 62.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 50f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "two-thirds";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 37.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "one-third";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 25f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 12.5f) {
			healthAnimation0.Animation = "two-thirds";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 0f) {
			healthAnimation0.Animation = "one-third";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else {
			healthAnimation0.Animation = "empty";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
	}

	private Label scoreLabel;
	private AnimatedSprite2D healthAnimation0;
	private AnimatedSprite2D healthAnimation1;
	private AnimatedSprite2D healthAnimation2;
}

using General;
using Godot;
using System;

public partial class HologramGadget : Area2D
{
	[Signal]
	public delegate void HologramFinishedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Animation = "load";
		animation.Frame = 0;

		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		collision.Disabled = true;

		hologramSound = GetNode<AudioStreamPlayer>("HologramSound");
		hologramReverseSound = GetNode<AudioStreamPlayer>("HologramReverseSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = position;
	}

	public void SetPosition(Vector2 playerPos) {
		collision.Disabled = false;
		animation.Animation = "load";
		lives = 10;
		animation.Frame = 0;
		float yPos = (Math.Abs((int)GD.Randi()) % 185) + 16;
		while(Math.Abs(yPos - playerPos.Y) < 32) {
			yPos = (Math.Abs((int)GD.Randi()) % 185) + 16;
		}
		GlobalPosition = new Vector2(playerPos.X, yPos);
		position = GlobalPosition;
		animation.Play();
		if(GameData.Instance.GetCanPlaySFX()) hologramSound.Play();
	}

	private void OnAreaEntered(Area2D area)
	{
		if(Visible && animation.Animation.Equals("hologram") && area is EnemyBullet) {
			lives--;
			if(lives <= 0) {
				animation.Animation = "erase";
				animation.Frame = 0;
				animation.Play();
				if(GameData.Instance.GetCanPlaySFX()) hologramReverseSound.Play();
			}
			else {
				animation.Frame = lives - 1;
			}
		}
	}	

	private void OnAnimationFinished()
	{
		if(animation.Animation.Equals("load")) {
			animation.Stop();
			animation.Animation = "hologram";
			animation.Frame = lives - 1;
		}
		else if(animation.Animation.Equals("erase")) {
			collision.Disabled = true;
			EmitSignal(SignalName.HologramFinished);
		}
	}

	private AnimatedSprite2D animation;
	private int lives = 10;
	private Vector2 position;
	private CollisionShape2D collision;
	private AudioStreamPlayer hologramSound;
	private AudioStreamPlayer hologramReverseSound;
}

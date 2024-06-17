using General;
using Godot;
using System;

public partial class EnemyBullet : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		movement = new CharacterMovement(Position, _isNPC: true);
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		animation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			deltaSum = 0;
			delta = 0.0167f;
			movement.Update((float)delta);
			GlobalPosition = movement.GetPos();
			movement.SetPos(GlobalPosition);
		}
	}

	public void SetPoints(Vector2 start, Vector2 end) {
		GD.Print("Fire");
		Vector2 vel = new Vector2(end.X - start.X, end.Y - start.Y);
		float norm = (float)Math.Sqrt((vel.X * vel.X) + (vel.Y * vel.Y));
		float scale = 100f;
		vel.X *= scale/norm;
		vel.Y *= scale/norm;

		GlobalPosition = start;
		movement.SetPos(GlobalPosition);
		movement.SetGoalVel(vel);
		movement.SetVel(vel);
	}

	private void CheckBounds() {
		if(Position.X < 0 || Position.X > 256 || Position.Y < 0 || Position.Y > 224) QueueFree();
	}	

	private void OnAreaEntered(Area2D area)
	{
		if(area is PlayerChase) {
			GameData.Instance.DecrementHealth(10);
			QueueFree();
		}
	}

	private CharacterMovement movement;
	private AnimatedSprite2D animation;
	private double deltaSum = 0;
}

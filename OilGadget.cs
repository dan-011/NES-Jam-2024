using General;
using Godot;
using System;

public partial class OilGadget : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animation.Frame = 0;

		movement = new CharacterMovement(Position, _isNPC: true);
		movement.SetGoalVel(new Vector2(horizontalVel, 20));
		movement.SetVel(new Vector2(horizontalVel, 0));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameData.Instance.GetIsPaused()) return;
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			delta = deltaSum;
			deltaSum = 0;
			//delta = 0.0167f;
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
		}
		HandleBounds();
	}
	
	public void SetStartPos(Vector2 pos) {
		Position = pos;
		movement.SetPos(pos);
		animation.Play();
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if(area is NPC && !((area as NPC).IsOiled())) {
			(area as NPC).Oil();
			QueueFree();
		}
	}

	private void HandleBounds() {
		if(Position.X < 0 || Position.X > 256 || Position.Y < 0 || Position.Y > 224) {
			QueueFree();
		}
	}

	private CharacterMovement movement;
	private AnimatedSprite2D animation;
	private double deltaSum = 0;
	private float horizontalVel = -135f;
}

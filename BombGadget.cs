using General;
using Godot;
using System;
using static General.CharacterMovement;

public partial class BombGadget : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("BombAnimation");
		movement = new CharacterMovement(Position, _isNPC: true);
		collisionShape = GetNode<CollisionShape2D>("BombCollisionShape");
		movement.SetGoalVel(new Vector2(-80, 20));
		movement.SetVel(new Vector2(-80, 0));

		animation.Frame = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(animation.Frame == 3) collisionShape.Disabled = false;
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			if(animation.Frame == 3 && movement.GetVel().Y != 0) {
				movement.SetVel(new Vector2(-80, 0));
				movement.SetGoalVel(new Vector2(-80, 0));
			}
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
		}
	}

	public void SetStartPos(Vector2 pos) {
		Position = pos;
		movement.SetPos(Position);
		animation.Play();
	}
	
	
	private void OnBombAnimationFinished()
	{
		QueueFree();
	}


	
	private AnimatedSprite2D animation;
	private CharacterMovement movement;
	private CollisionShape2D collisionShape;
}

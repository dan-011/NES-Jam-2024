using General;
using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using static General.CharacterMovement;


public partial class PlayerChase : Area2D
{
	// we'll create a parallax animation that will be constantly running in the background
	// the player can boost forward but then it pushes them back (similar to jumping)
	// otherwise changes in the x direction are locked
	// we should have an idle bobbing of the character while moving in a straight line
	
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		Vector2 startPos = new Vector2(Position.X, 100);
		Position = startPos;
		movement = new CharacterMovement(startPos, 400, 800);
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		screenSize = GetViewportRect().Size;

		
		animation.Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		InputHandling();
		deltaSum += delta;
		if(deltaSum >= 0.0167f) {
			deltaSum = 0;
			delta = 0.0167f;
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
			// Vector2 globalCenter = new Vector2(GlobalPosition.X + collisionShape.Position.X + collisionShape)
			GameData.Instance.SetPlayerPos(GlobalPosition);
			HandleBounds();
			AnimationHandling();
		}
	}
	public CharacterMovement GetPlayerMovement() {
		return movement;
	}

	private void InputHandling() {
		if(Input.IsActionJustReleased("move_up")) {
			movement.ReleaseY();
		}
		if(Input.IsActionJustReleased("move_down")) {
			movement.ReleaseY();
		}
		if(Input.IsActionPressed("move_up")) {
			if(canGoUp) movement.MoveUp();
		}
		if(Input.IsActionPressed("move_down")) {
			if(canGoDown) movement.MoveDown();
		}
		if(Input.IsActionJustPressed("boost")) {
			movement.Boost();
		}
		else if(!movement.GetIsBoosting() && Input.IsActionJustPressed("gadget")) {
			isThrowing = true;
			animation.Stop();
			animation.Play("throw");
		}
	}

	private void AnimationHandling() {
		if(movement.GetIsBoosting()) {
			if(!animation.Animation.Equals("boost")) {
				animation.Stop();
				animation.Play("boost");
				GD.Print("boost");
			}
		}
		else if(!isThrowing) {
			if(movement.GetVel().Y == 0 && !animation.Animation.Equals("idle")) {
				animation.Stop();
				animation.Play("idle");
			}
			else if(movement.GetVel().Y < 0 && !animation.Animation.Equals("up")) {
				animation.Stop();
				animation.Play("up");
			}
			else if(movement.GetVel().Y > 0 && !animation.Animation.Equals("down")) {
				animation.Stop();
				animation.Play("down");
			}
		}
	}

	private void HandleBounds() {
		Vector2 dims = collisionShape.Shape.GetRect().Size;
		if(Position.Y < (dims.Y / 2) + 5) {
			canGoUp = false;
			movement.SetVel(new Vector2(movement.GetVel().X, Math.Max(0, movement.GetVel().Y)));
		}
		else canGoUp = true;
		if(Position.Y > screenSize.Y - (dims.Y / 2) - 5) {
			canGoDown = false;
			movement.SetVel(new Vector2(movement.GetVel().X, Math.Min(movement.GetVel().Y, 0)));
		}
		else canGoDown = true;
	}
	
	private void OnAreaEntered(Area2D area)
	{
		GD.Print("Player hit");
	}
	
		
	private void OnAnimationFinished()
	{
		if(animation.Animation.Equals("throw")) {
			isThrowing = false;
		}
	}


	private CharacterMovement movement;
	private Vector2 screenSize;
	private CollisionShape2D collisionShape;
	private AnimatedSprite2D animation;
	private bool canGoDown = true;
	private bool canGoUp = true;
	private bool isThrowing = false;
	private double deltaSum = 0;

}

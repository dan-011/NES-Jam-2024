using General;
using Godot;
using System;
using System.ComponentModel.DataAnnotations;
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
		Vector2 startPos = new Vector2(Position.X - 20, 100);
		Position = startPos;
		movement = new CharacterMovement(startPos, 400, 400);
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		screenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			InputHandling();
			movement.Update((float)delta);
			Position = movement.GetPos();
			movement.SetPos(Position);
			HandleBounds();
		}
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

	private CharacterMovement movement;
	private Vector2 screenSize;
	private CollisionShape2D collisionShape;
	private bool canGoDown = true;
	private bool canGoUp = true;
}

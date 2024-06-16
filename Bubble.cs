using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class Bubble : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bubbleAnimation = GetNode<AnimatedSprite2D>("CompositeSprites/Bubble");
		bubbleAnimation.Animation = "bubble";
		int chooseGadget = Math.Abs((int)GD.Randi()) % (GetNode<Node2D>("CompositeSprites").GetChildCount() - 1);
		List<string> gadgets = new List<string>();
		gadgets.Add("CompositeSprites/SmokeBomb");
		gadgetAnimation = GetNode<AnimatedSprite2D>(gadgets[chooseGadget]);
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		label = GetNode<Label>("Label");
		label.Text = ((Math.Abs((int)GD.Randi()) % 5) + 1).ToString();
		
		Position = new Vector2(256, (Math.Abs((int)GD.Randi()) % 185) + 32);

		movement = new CharacterMovement(Position, _isNPC: true);
		float startXVel = -1 * ((Math.Abs((int)GD.Randi()) % 50) + 20);
		float startYVel = (int)GD.Randi() % 50;
		startYVel = startYVel < 0 ? startYVel - 20 : startYVel + 20;
		Vector2 startVel = new Vector2(startXVel, startYVel);

		movement.SetGoalVel(startVel);
		movement.SetVel(startVel);

		gadgetAnimation.Play();
		bubbleAnimation.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			movement.Update((float)delta);
			Position = movement.GetPos();
			HandleBounds();
		}
	}

	private void HandleBounds() {
		if(Position.Y < 16 || Position.Y + 16 > 224) {
			Vector2 vel = movement.GetVel();
			vel.Y *= -1;
			movement.SetGoalVel(vel);
			movement.SetVel(vel);
		}
		if(Position.X < -16) QueueFree();
	}
	
	
	private void OnAreaEntered(Area2D area)
	{
		if(area is PlayerChase) {
			GD.Print("Player received gadget");
			gadgetAnimation.Visible = false;
			label.Visible = false;
			bubbleAnimation.Stop();
			bubbleAnimation.Play("pop");
		}
	}

	
	private void OnBubbleAnimationFinished()
	{
		if(bubbleAnimation.Animation == "pop") QueueFree();
	}

	private AnimatedSprite2D bubbleAnimation;
	private AnimatedSprite2D gadgetAnimation;
	private CollisionShape2D collisionShape;
	private Label label;
	private CharacterMovement movement;
	private Timer spawnTimer;
}

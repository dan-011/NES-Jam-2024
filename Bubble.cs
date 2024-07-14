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
		gadgetAnimation = GetNode<AnimatedSprite2D>("CompositeSprites/Item");
		chooseGadget = Math.Abs((int)GD.Randi()) % gadgetAnimation.SpriteFrames.GetFrameCount("default");
		gadgetAnimation.Frame = chooseGadget;
		//if(tutorialGadgetCount == 0 && doingTutorial) chooseGadget = 1;
		//else if(tutorialGadgetCount == 1 && doingTutorial) chooseGadget = 0;
		gadgetAnimation.Visible = true;

		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		popSound = GetNode<AudioStreamPlayer>("PopSound");

		label = GetNode<Label>("Label");
		amount = (Math.Abs((int)GD.Randi()) % 5) + 1;
		label.Text = amount.ToString();

		int yPos = (Math.Abs((int)GD.Randi()) % 180) + 32;
		
		Position = new Vector2(256, yPos);

		movement = new CharacterMovement(Position, _isNPC: true);
		float startXVel = -1 * ((Math.Abs((int)GD.Randi()) % 50) + 20);
		float startYVel = (int)GD.Randi() % 50;
		startYVel = startYVel < 0 ? startYVel - 20 : startYVel + 20;
		Vector2 startVel = new Vector2(startXVel, startYVel);

		movement.SetGoalVel(startVel);
		movement.SetVel(startVel);

		bubbleAnimation.Play();
	}

	public void SetCustomItem(int item) {
		chooseGadget = item;
		gadgetAnimation.Frame = chooseGadget;
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
			gadgetAnimation.Visible = false;
			label.Visible = false;
			bubbleAnimation.Stop();
			bubbleAnimation.Play("pop");
			if(GameData.Instance.GetCanPlaySFX()) popSound.Play();
			if(GameData.Instance.GetTotalItems() == 0) GameData.Instance.SetSelectedGadget(chooseGadget);
			GameData.Instance.AddToInventory(chooseGadget, (uint)amount);
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
	private double deltaSum = 0;
	private int amount;
	private int chooseGadget;
	private AudioStreamPlayer popSound;
}

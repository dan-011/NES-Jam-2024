using Godot;
using System;

public partial class Intro : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cycle = GetNode<AnimatedSprite2D>("Cycle");
		person = GetNode<AnimatedSprite2D>("Person");
		lightning = GetNode<AnimatedSprite2D>("Lightning");
		shoes = GetNode<AnimatedSprite2D>("Shoes");
		timer = GetNode<Timer>("Timer");
		blinkTimer = GetNode<Timer>("BlinkTimer");
		titleLabel = GetNode<Label>("TitleLabel");
		startLabel = GetNode<Label>("StartLabel");
		blinkThreshold = 0.5f;

		mainMenu = GetNode<MainMenu>("MainMenu");
		Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		deltaSum += delta;
		if(!titleLabel.Visible && lightning.Frame == 10 && person.Animation.Equals("enter")) {
			titleLabel.Visible = true;
			titleLabel.AddThemeColorOverride("font_color", new Color("bcbcbc"));
			startLabel.Visible = true;
			blinkTimer.Start(blinkThreshold);
		}
		if(titleLabel.Visible) {
			InputHandling();
		}
		if(person.Animation.Equals("exit") && titleLabel.GlobalPosition.Y < 75 && deltaSum >= 1/25) {
			deltaSum = 0;
			Vector2 globalPos = titleLabel.GlobalPosition;
			globalPos.Y += 1;
			titleLabel.GlobalPosition = globalPos;
		}
	}

	public void Start() {
		startLabel.Visible = false;
		titleLabel.Visible = false;
		person.Frame = 0;
		lightning.Animation = "enter";
		lightning.Frame = 0;
		shoes.Frame = 0;
		cycle.Animation = "begin";
		cycle.Frame = 0;
		cycle.Play();
		person.Animation = "enter";
		person.Frame = 0;
	}

	private void InputHandling() {
		if(Input.IsActionJustPressed("start")) {
			blinkTimer.Stop();
			blinkThreshold = 0.1f;
			blinkTimer.Start(blinkThreshold);
			timer.Start(1);
		}
	}
		

	private void OnCycleAnimationFinished()
	{
		if(cycle.Animation.Equals("begin")) {
			cycle.Animation = "cycle";
			cycle.Frame = 0;
			cycle.Play();
			timer.Start(blinkThreshold);
		}
	}


	private void OnPersonAnimationFinished()
	{
		if(person.Animation.Equals("enter")) timer.Start(0.05);
		else {
			mainMenu.Visible = true;
		}
	}

	
	private void OnTimerTimeout()
	{
		if(person.Frame == 0) {
			timer.Stop();
			person.Play();
		}
		else if(lightning.Frame == 0) {
			timer.Stop();
			lightning.Play();
		}
		else {
			timer.Stop();
			blinkTimer.Stop();
			startLabel.Visible = false;
			//titleLabel.Visible = false;
			lightning.Stop();
			lightning.Animation = "exit";
			lightning.Frame = 0;
			lightning.Play();
			shoes.Play();
			person.Animation = "exit";
			person.Stop();
			person.Frame = 0;
			person.Play();
		}
	}

	
	private void OnBlinkTimerTimeout()
	{
		startLabel.Visible = !startLabel.Visible;
		blinkTimer.Start(blinkThreshold);
	}


	private AnimatedSprite2D cycle;
	private AnimatedSprite2D person;
	private AnimatedSprite2D lightning;
	private AnimatedSprite2D shoes;
	private Timer timer;
	private Timer blinkTimer;
	private Label titleLabel;
	private Label startLabel;
	private float blinkThreshold;
	private double deltaSum = 0;
	private MainMenu mainMenu;
}

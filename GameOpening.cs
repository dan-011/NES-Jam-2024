using General;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class GameOpening : CanvasLayer
{
	[Signal]
	public delegate void GameOpeningEndedEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backgroundSlide = GetNode<AnimatedSprite2D>("BackgroundSlide");
		backgroundBegin = GetNode<AnimatedSprite2D>("BackgroundBegin");
		tempPlayer = GetNode<AnimatedSprite2D>("TempPlayer");
		timer = GetNode<Timer>("Timer");

		Vector2 pos = new Vector2(-30, 100);
		movement = new CharacterMovement(pos, _isNPC: true);
		int vel = 50;
		movement.SetGoalVel(new Vector2(vel, 0));
		movement.SetVel(new Vector2(vel, 0));
		tempPlayer.GlobalPosition = pos;

		backgroundSlide.Frame = 0;
		backgroundBegin.Frame = 0;
		tempPlayer.Frame = 0;

		paragraph = 0;
		sentence = 0;
		letter = 0;

		aLabel = GetNode<Label>("ALabel");
		aLabel.Text = "Press " + GameData.Instance.GetControllerMapping("A") + " to continue";

		startLabel = GetNode<Label>("StartLabel");
		startLabel.Text = "Press " + GameData.Instance.GetControllerMapping("Start") + " to skip";

		descriptionLabel = GetNode<Label>("DescriptionLabel");
		descriptionLabel.Text = "";

		description = new List<List<string>>();
		description.Add(new List<string>());
		description.Add(new List<string>());
		description.Add(new List<string>());
		description.Add(new List<string>());
		description.Add(new List<string>());
		description.Add(new List<string>());

		description[0].Add("The  year  is  2311,  and  the  media");
		description[0].Add("conglomerate  BraxCorp  has  slowly");
		description[0].Add("monopolized  every  industry  and");
		description[0].Add("taken  control  of  every  government...");
		
		description[1].Add("The  world  has  fallen  victim  to");
		description[1].Add("this  corporation,  with  prices  for");
		description[1].Add("basic  ammenities  skyrocketting  and  no");
		description[1].Add("relief  in  sight...");

		description[2].Add("While  the  poor  struggle  to  survive,");
		description[2].Add("BraxCorp  just  becomes  more  rich  and");
		description[2].Add("powerful,  carelessly overlooking  the");
		description[2].Add("chaos  in  their  obsidian  tower,");
		description[2].Add("The  Obelisk...");

		description[3].Add("As  part  of  a  group  of  freedom  fighters");
		description[3].Add("resisting  BraxCorp,  you  have  acted  to");
		description[3].Add("sabotage  the  company  to  someday  bring");
		description[3].Add("power  back  to  the  people...");

		description[4].Add("Today,  you  broke  into  the  Obelisk");
		description[4].Add("and  gathered  a  piece  of  information");
		description[4].Add("which might put down  BraxCorp  for");
		description[4].Add("good,  but  you  set  off  an  alarm,");
		description[4].Add("alerting  the  guards  in  the  process...");

		description[5].Add("It  is  up  to  you  to  get  back  to  the");
		description[5].Add("freedom  fighters  with  the  information");
		description[5].Add("you  have  collected  and  free  the  world");
		description[5].Add("from  this  rule.  Now  get  your  gadget");
		description[5].Add("pouch,  your  rocket  boots,  and");
		description[5].Add("HERE  WE  GO!!!");

		textTimer = GetNode<Timer>("TextTimer");
		
		descriptionMusic = GetNode<AudioStreamPlayer>("DescriptionMusic");

		tempPlayer.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible) return;
		InputHandling();
		deltaSum += delta;
		if(backgroundSlide.Frame == 24 && !textFinished && backgroundSlide.IsPlaying()) {
			if(GameData.Instance.GetCanPlayMusic()) descriptionMusic.Play();
			backgroundSlide.Pause();
			textTimer.Start(0.01);
			startLabel.Visible = true;
			descriptionLabel.Visible = true;
			timer.Stop();
		}
		if(deltaSum >= 0.0167) {
			delta = deltaSum;			
			deltaSum = 0;
			//delta = 0.0167f;
			if(sendPlayer) {
				movement.Update((float)delta);
				tempPlayer.GlobalPosition = movement.GetPos();
				if(Math.Ceiling(tempPlayer.GlobalPosition.X) >= 158) {
					Vector2 pos = tempPlayer.GlobalPosition;
					pos.X = 158;
					tempPlayer.GlobalPosition = pos;
					sendPlayer = false;
				}
			}
		}
		if(startGame && tempPlayer.GlobalPosition.X >= 158) {
			if(backgroundBegin.Frame == backgroundBegin.SpriteFrames.GetFrameCount("default") - 1) {
				startGame = false;
				EmitSignal(SignalName.GameOpeningEnded);
			}
		}
	}

	public void Play() {
		backgroundSlide.Play();
		//if(GameData.Instance.GetCanPlayMusic()) descriptionMusic.Play();	
	}

	public void InputHandling() {
		if(Input.IsActionJustPressed(GameData.Instance.GetA())) {
			Input.ActionRelease(GameData.Instance.GetA());
			if(!textFinished) NextParagraph();
		}
		if(Input.IsActionJustPressed(GameData.Instance.GetB())) {
			Input.ActionRelease(GameData.Instance.GetB());
			if(!textFinished) NextParagraph();
		}
		if(Input.IsActionJustPressed("start")) {
			Input.ActionRelease("start");
			if(!textFinished) SkipText();
		}
	}

	private void NextParagraph() {
		if(paragraph == description.Count) {
			SkipText();
		}
		
		if(paragraph < description.Count && sentence < description[paragraph].Count && letter > 0) {
			textTimer.Stop();
			descriptionLabel.Text = "";
			for(int i = 0; i < description[paragraph].Count; i++) {
				descriptionLabel.Text += description[paragraph][i] + "\n";
			}
			paragraph++;
			letter = 0;
			sentence = 0;
			aLabel.Visible = true;
		}
		else {
			descriptionLabel.Text = "";
			aLabel.Visible = false;
			textTimer.Start(0.01);
		}
	}

	private void SkipText() {
		startLabel.Visible = false;
		aLabel.Visible = false;
		descriptionLabel.Visible = false;
		textFinished = true;
		backgroundSlide.Play();
		descriptionMusic.Stop();
		textTimer.Stop();
		timer.Start(1);
	}
	
	private void OnBackgroundSlideAnimationFinished()
	{
		backgroundSlide.Stop();
		backgroundSlide.Visible = false;
		timer.Start(1);
		backgroundBegin.Visible = true;
		backgroundBegin.Play();
		sendPlayer = true;
	}
		
	private void OnTimerTimeout()
	{
		backgroundBegin.SpeedScale += 1f;
		//if(!sendPlayer && tempPlayer.GlobalPosition.X == -16 && backgroundBegin.SpeedScale >= 1f && backgroundSlide.Frame == backgroundSlide.SpriteFrames.GetFrameCount("slide")) {
		//	sendPlayer = true;
		//}
		if(backgroundBegin.SpeedScale >= 4f) {
			timer.Stop();
			startGame = true;
		}
		else {
			timer.Start(1);
		}
	}
	
	private void OnTextTimerTimeout()
	{
		textTimer.Stop();
		if(paragraph < description.Count && sentence < description[paragraph].Count && letter < description[paragraph][sentence].Length) {
			descriptionLabel.Text += description[paragraph][sentence][letter];
		}
		else return;
		letter++;
		if(letter == description[paragraph][sentence].Length) {
			descriptionLabel.Text += "\n";
			letter = 0;
			sentence++;
		}
		if(sentence == description[paragraph].Count) {
			sentence = 0;
			paragraph++;
			aLabel.Visible = true;
		}
		else {
			textTimer.Start(0.01);
		}
	}

	private void OnDescriptionMusicFinished()
	{
		if(GameData.Instance.GetCanPlayMusic()) descriptionMusic.Play();
	}

	private AnimatedSprite2D backgroundSlide;
	private AnimatedSprite2D backgroundBegin;
	private AnimatedSprite2D tempPlayer;
	private CharacterMovement movement;
	private Timer timer;
	private double deltaSum = 0;
	private bool sendPlayer = false;
	private bool startGame =  false;
	private List<List<string>> description;
	private bool textFinished = false;
	private Timer textTimer;
	private int paragraph;
	private int sentence;
	private int letter;
	private Label aLabel;
	private Label startLabel;
	private Label descriptionLabel;
	private AudioStreamPlayer descriptionMusic;
}

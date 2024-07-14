using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class TutorialDialog : CanvasLayer
{
	[Signal]
	public delegate void ContinueTutorialEventHandler();
	[Signal]
	public delegate void SkipTutorialEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tutorialText = new List<string>();
		tutorialText.Add("Use the " + GameData.Instance.GetControllerMapping("D-Pad") + " to move up and down");
		tutorialText.Add("Press " + GameData.Instance.GetControllerMapping("B") + " to boost forward");
		tutorialText.Add("Touch the bubble to get the gadget");
		tutorialText.Add("Press " + GameData.Instance.GetControllerMapping("Select") + " to change the gadget");
		tutorialText.Add("Press " + GameData.Instance.GetControllerMapping("A") + " to use the selected item");

		waitTime = new List<float>();
		waitTime.Add(3);
		waitTime.Add(3);
		waitTime.Add(5);
		waitTime.Add(6);

		currentInstruction = tutorialText.Count;

		dialogText = GetNode<Label>("DialogText");
		aLabel = GetNode<Label>("ALabel");
		startLabel = GetNode<Label>("StartLabel");
		aLabel.Text = "Press " + GameData.Instance.GetControllerMapping("A") + " to continue";
		startLabel.Text = "Press " + GameData.Instance.GetControllerMapping("Start") + " to skip tutorial";
	}

	public void Open() {
		Visible = true;
		currentInstruction = 0;
		dialogText.Text = tutorialText[currentInstruction];
		currentInstruction++;
		GetTree().Paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible) {
			InputHandling();
		}
	}

	public void NextInstruction() {
		GetTree().Paused = true;
		dialogText.Text = tutorialText[currentInstruction];
		currentInstruction++;
		Visible = true;
	}
	public float GetWaitTime() {
		return waitTime[currentInstruction-1];
	}
	public bool HasInstructionsLeft() {
		return currentInstruction < tutorialText.Count;
	}
	public bool StartInstructions() {
		return currentInstruction == tutorialText.Count;
	}
	public int GetCurrentInstruction() {
		return currentInstruction;
	}
	private void OpenInstruction() {
		GetTree().Paused = true;
		dialogText.Text = tutorialText[currentInstruction];
		Visible = true;
	}
	public void BoostInstruction() {
		currentInstruction = 1;
		OpenInstruction();
	}

	public void BubbleInstruction() {
		currentInstruction = 2;
		OpenInstruction();
	}
	public void ChangeGadgetInstruction() {
		currentInstruction = 3;
		OpenInstruction();
	}
	public void UseItemInstruction() {
		currentInstruction = 4;
		OpenInstruction();
		currentInstruction++;
	}
	private void InputHandling() {
		if(Input.IsActionJustPressed(GameData.Instance.GetA())) {
			EmitSignal(SignalName.ContinueTutorial);
		}
		if(Input.IsActionJustPressed("start")) {
			EmitSignal(SignalName.SkipTutorial);
		}
	}
	private List<string> tutorialText;
	private List<float> waitTime;
	private int currentInstruction;
	private Label dialogText;
	private Label aLabel;
	private Label startLabel;
}

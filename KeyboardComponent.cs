using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class KeyboardComponent : CanvasLayer
{
	[Signal]
	public delegate void SubmitTextEventHandler(string text);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectables = new List<List<Label>>();
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());
		selectables.Add(new List<Label>());

		selectables[0].Add(GetNode<Label>("LabelA"));
		selectables[0].Add(GetNode<Label>("LabelB"));
		selectables[0].Add(GetNode<Label>("LabelC"));
		selectables[0].Add(GetNode<Label>("LabelD"));
		selectables[0].Add(GetNode<Label>("LabelE"));
		selectables[0].Add(GetNode<Label>("LabelF"));
		selectables[0].Add(GetNode<Label>("LabelG"));
		selectables[0].Add(GetNode<Label>("LabelH"));
		selectables[0].Add(GetNode<Label>("LabelI"));

		selectables[1].Add(GetNode<Label>("LabelJ"));
		selectables[1].Add(GetNode<Label>("LabelK"));
		selectables[1].Add(GetNode<Label>("LabelL"));
		selectables[1].Add(GetNode<Label>("LabelM"));
		selectables[1].Add(GetNode<Label>("LabelN"));
		selectables[1].Add(GetNode<Label>("LabelO"));
		selectables[1].Add(GetNode<Label>("LabelP"));
		selectables[1].Add(GetNode<Label>("LabelQ"));
		selectables[1].Add(GetNode<Label>("LabelR"));

		selectables[2].Add(GetNode<Label>("LabelS"));
		selectables[2].Add(GetNode<Label>("LabelT"));
		selectables[2].Add(GetNode<Label>("LabelU"));
		selectables[2].Add(GetNode<Label>("LabelV"));
		selectables[2].Add(GetNode<Label>("LabelW"));
		selectables[2].Add(GetNode<Label>("LabelX"));
		selectables[2].Add(GetNode<Label>("LabelY"));
		selectables[2].Add(GetNode<Label>("LabelZ"));
		selectables[2].Add(GetNode<Label>("LabelSpace1"));

		selectables[3].Add(GetNode<Label>("LabelExcl"));
		selectables[3].Add(GetNode<Label>("LabelQues"));
		selectables[3].Add(GetNode<Label>("LabelDollar"));
		selectables[3].Add(GetNode<Label>("LabelAmp"));
		selectables[3].Add(GetNode<Label>("LabelPlus"));
		selectables[3].Add(GetNode<Label>("LabelStar"));
		selectables[3].Add(GetNode<Label>("LabelDash"));
		selectables[3].Add(GetNode<Label>("LabelUScr"));
		selectables[3].Add(GetNode<Label>("LabelEq"));

		selectables[4].Add(GetNode<Label>("LabelAt"));
		selectables[4].Add(GetNode<Label>("LabelPerc"));
		selectables[4].Add(GetNode<Label>("LabelXor"));
		selectables[4].Add(GetNode<Label>("LabelOr"));
		selectables[4].Add(GetNode<Label>("LabelColon"));
		selectables[4].Add(GetNode<Label>("LabelSColon"));
		selectables[4].Add(GetNode<Label>("LabelPer"));
		selectables[4].Add(GetNode<Label>("LabelComm"));
		selectables[4].Add(GetNode<Label>("LabelTilde"));

		selectables[5].Add(GetNode<Label>("LabelParenL"));
		selectables[5].Add(GetNode<Label>("LabelParenR"));
		selectables[5].Add(GetNode<Label>("LabelSBL"));
		selectables[5].Add(GetNode<Label>("LabelSBR"));
		selectables[5].Add(GetNode<Label>("LabelFSlsh"));
		selectables[5].Add(GetNode<Label>("LabelBSlsh"));
		selectables[5].Add(GetNode<Label>("LabelLBrack"));
		selectables[5].Add(GetNode<Label>("LabelRBrack"));
		selectables[5].Add(GetNode<Label>("LabelSpace2"));

		selectables[6].Add(GetNode<Label>("LabelCase"));
		selectables[6].Add(GetNode<Label>("LabelBack"));
		selectables[6].Add(GetNode<Label>("LabelEnter"));

		bottomSelectors = new List<AnimatedSprite2D>();
		bottomSelectors.Add(GetNode<AnimatedSprite2D>("CaseSelect"));
		bottomSelectors.Add(GetNode<AnimatedSprite2D>("BackSelect"));
		bottomSelectors.Add(GetNode<AnimatedSprite2D>("EnterSelect"));

		text = GetNode<Label>("Text");
		letterSelect = GetNode<AnimatedSprite2D>("LetterSelect");
		selectTimer = GetNode<Timer>("SelectTimer");
		timerFlicker = GetNode<Timer>("TimerFlicker");

		topLeft = new Vector2(60, 76);
		bottomRight =  new Vector2(180, 151);
		menuTick = GetNode<AudioStreamPlayer>("MenuTick");
	}

	public void Open() {
		row = 0;
		col = 0;

		for(int i = 0; i < selectables.Count; i++) {
			for(int j = 0; j < selectables[i].Count; j++) {
				selectables[i][j].AddThemeColorOverride("font_color", new Color("bcbcbc"));
			}
		}

		text.Text = "_";
		letterSelect.Visible = true;
		ResetBottomSelectors();
		letterSelect.GlobalPosition = topLeft;
		canMove = true;
		showUnderscore = true;
		ToggleSelect(row, col);
		timerFlicker.Start(0.3);
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && canMove) {
			InputHandling();
		}
	}

	private void InputHandling() {
		if(Input.IsActionJustPressed("right")) {
			ToggleSelect(row, col);
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			col++;
			if(col == selectables[row].Count) {
				col = 0;
				if(AtBottom()) {
					ResetBottomSelectors();
					bottomSelectors[col].Visible = true;
				}
				else letterSelect.GlobalPosition = new Vector2(topLeft.X, letterSelect.GlobalPosition.Y);
			}
			else {
				if(AtBottom()) {
					ResetBottomSelectors();
					bottomSelectors[col].Visible = true;
				}
				else letterSelect.GlobalPosition = new Vector2(letterSelect.GlobalPosition.X + 15, letterSelect.GlobalPosition.Y);
			}
			ToggleSelect(row, col);
		}
		if(Input.IsActionJustPressed("left")) {
			ToggleSelect(row, col);
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			col--;
			if(col < 0) {
				col = selectables[row].Count - 1;
				if(AtBottom()) {
					ResetBottomSelectors();
					bottomSelectors[col].Visible = true;
				}
				else letterSelect.GlobalPosition = new Vector2(bottomRight.X, letterSelect.GlobalPosition.Y);
			}
			else {
				if(AtBottom()) {
					ResetBottomSelectors();
					bottomSelectors[col].Visible = true;
				}
				else letterSelect.GlobalPosition = new Vector2(letterSelect.GlobalPosition.X - 15, letterSelect.GlobalPosition.Y);
			}
			ToggleSelect(row, col);
		}
		if(Input.IsActionJustPressed("move_up")) {
			ToggleSelect(row, col);
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			bool wasAtBottom = false;
			if(AtBottom()) {
				wasAtBottom = true;
				ResetBottomSelectors();
			}
			row--;
			letterSelect.Visible = true;
			if(row < 0) { // at bottom
				row = selectables.Count - 1;
				letterSelect.Visible = false;
				col = CalculateBottomOption();
				bottomSelectors[col].Visible = true;
			}
			else if(wasAtBottom) {
				col = CalculateFromBottom();
				letterSelect.GlobalPosition = new Vector2(topLeft.X + (col * 15), bottomRight.Y);
			}
			else letterSelect.GlobalPosition = new Vector2(letterSelect.GlobalPosition.X, letterSelect.GlobalPosition.Y - 15);
			ToggleSelect(row, col);
		}
		if(Input.IsActionJustPressed("move_down")) {
			ToggleSelect(row, col);
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			if(AtBottom()) ResetBottomSelectors();
			row++;
			letterSelect.Visible = true;
			if(row == selectables.Count) { // was at bottom
				row = 0;
				col = CalculateFromBottom();
				Vector2 newPosition = topLeft;
				newPosition.X += col * 15;
				letterSelect.GlobalPosition = newPosition;
			}
			else if(AtBottom()) {
				letterSelect.Visible = false;
				col = CalculateBottomOption();
				bottomSelectors[col].Visible = true;
			}
			else letterSelect.GlobalPosition = new Vector2(letterSelect.GlobalPosition.X, letterSelect.GlobalPosition.Y + 15);
			ToggleSelect(row, col);
		}
		if(Input.IsActionPressed(GameData.Instance.GetA())) {
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			Input.ActionRelease(GameData.Instance.GetA());
			StartTimer();
		}
		else if(Input.IsActionJustPressed(GameData.Instance.GetB())) {
			if(GameData.Instance.GetCanPlaySFX() && GetRealTextLength() > 0) menuTick.Play();
			Backspace();
		}
		else if(Input.IsActionJustPressed("start") && GetRealTextLength() > 0) {
			if(GameData.Instance.GetCanPlaySFX()) menuTick.Play();
			SendText();
		}
	}

	private int GetRealTextLength() {
		bool prevUnderscore = showUnderscore;
		SetUnderscoreVisibility(false);
		int length = text.Text.Length;
		SetUnderscoreVisibility(prevUnderscore);
		return length;
	}

	private void SendText() {
		timerFlicker.Stop();
		SetUnderscoreVisibility(false);
		EmitSignal(SignalName.SubmitText, text.Text);
	}

	private void ToggleSelect(int i, int j) {
		if(selectables[i][j].GetThemeColor("font_color") == new Color("fcfcfc")) {
			selectables[i][j].AddThemeColorOverride("font_color", new Color("bcbcbc"));
		}
		else {
			selectables[i][j].AddThemeColorOverride("font_color", new Color("fcfcfc"));
		}
	}

	private int CalculateBottomOption() {
		int column = 0;
		if(col < 3) column = 0;
		else if(col < 6) column = 1;
		else if(col < 9) column = 2;
		return column;
	}

	private int CalculateFromBottom() {
		int column = 0;
		if(col == 0) column = 0;
		else if(col == 1) column = 3;
		else if(col == 2) column = 6;
		return column;
	}

	private void ResetBottomSelectors() {
		for(int i = 0; i < bottomSelectors.Count; i++) {
			bottomSelectors[i].Visible = false;
		}
	}

	private bool AtBottom() {
		return row == selectables.Count - 1;
	}

	private void StartTimer() {
		selectTimer.Start(0.1);
		canMove = false;
		letterSelect.Visible = false;
		ResetBottomSelectors();
		ToggleSelect(row, col);
	}
	private void Backspace() {
		if(text.Text.Length > 0 && !showUnderscore) {
			text.Text = text.Text.Substring(0, text.Text.Length - 1);
		}
		else if(text.Text.Length > 1 && showUnderscore) {
			SetUnderscoreVisibility(false);
			text.Text = text.Text.Substring(0, text.Text.Length - 1);
			SetUnderscoreVisibility(true);
		}
		if(timerFlicker.IsStopped() && text.Text.Length < 18) timerFlicker.Start(0.3);
	}

	private void OnSelectTimerTimeout()
	{
		selectTimer.Stop();
		if(row == selectables.Count - 1) {
			if(col == 0) {
				if(selectables[row][col].Text.Equals("UPPER")) {
					selectables[row][col].Text = "lower";
					for(int i = 0; i < selectables.Count-1; i++) {
						for(int j = 0; j < selectables[i].Count; j++) {
							selectables[i][j].Text = selectables[i][j].Text.ToUpper();
						}
					}
				}
				else {
					selectables[row][col].Text = "UPPER";
					for(int i = 0; i < selectables.Count-1; i++) {
						for(int j = 0; j < selectables[i].Count; j++) {
							selectables[i][j].Text = selectables[i][j].Text.ToLower();
						}
					}
				}
				bottomSelectors[0].Visible = true;
			}
			else if(col == 1) {
				Backspace();
				bottomSelectors[col].Visible = true;
			}
			else {
				if(text.Text.Trim().Length > 0) {
					SendText();
					bottomSelectors[col].Visible = true;
				}
			}
		}
		else {
			if(text.Text.Length < 18 || (showUnderscore && text.Text.Length == 18)) {
				timerFlicker.Paused = true;
				SetUnderscoreVisibility(false);
				text.Text += selectables[row][col].Text[0];
				timerFlicker.Paused = false;
			}
			if((showUnderscore && text.Text.Length > 18) || (!showUnderscore && text.Text.Length == 18)) {
				timerFlicker.Stop();
				SetUnderscoreVisibility(false);
			}
			letterSelect.Visible = true;
		}
		ToggleSelect(row, col);
		canMove = true;
	}

	
	private void OnTimerFlickerTimeout()
	{
		timerFlicker.Stop();
		SetUnderscoreVisibility(!showUnderscore);
		timerFlicker.Start(0.3);
	}

	private void SetUnderscoreVisibility(bool visible) {
		if(!showUnderscore && visible) {
			showUnderscore = true;
			text.Text += '_';
		}
		else if(showUnderscore && !visible) {
			showUnderscore = false;
			text.Text = text.Text.Substring(0, text.Text.Length-1);
		}
	}


	private List<List<Label>> selectables;
	private List<AnimatedSprite2D> bottomSelectors;
	private Label text;
	private AnimatedSprite2D letterSelect;
	private Timer selectTimer;
	private int row;
	private int col;
	private bool canMove;
	private Vector2 topLeft;
	private Vector2 bottomRight;
	private Timer timerFlicker;
	private bool showUnderscore;
	private AudioStreamPlayer menuTick;
}

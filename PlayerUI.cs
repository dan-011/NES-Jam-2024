using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerUI : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		healthAnimation0 = GetNode<AnimatedSprite2D>("HealthAnimation0");
		healthAnimation1 = GetNode<AnimatedSprite2D>("HealthAnimation1");
		healthAnimation2 = GetNode<AnimatedSprite2D>("HealthAnimation2");

		itemSlots = new List<AnimatedSprite2D>();
		itemSelectors = new List<AnimatedSprite2D>();
		itemAmounts = new List<Label>();

		itemSlots.Add(GetNode<AnimatedSprite2D>("BombSlot"));
		itemSlots.Add(GetNode<AnimatedSprite2D>("ShieldSlot"));
		itemSlots.Add(GetNode<AnimatedSprite2D>("ReactorSlot"));
		itemSlots.Add(GetNode<AnimatedSprite2D>("OilSlot"));
		itemSlots.Add(GetNode<AnimatedSprite2D>("HologramSlot"));

		itemSelectors.Add(GetNode<AnimatedSprite2D>("BombSelector"));
		itemSelectors.Add(GetNode<AnimatedSprite2D>("ShieldSelector"));
		itemSelectors.Add(GetNode<AnimatedSprite2D>("ReactorSelector"));
		itemSelectors.Add(GetNode<AnimatedSprite2D>("OilSelector"));
		itemSelectors.Add(GetNode<AnimatedSprite2D>("HologramSelector"));

		itemAmounts.Add(GetNode<Label>("BombAmount"));
		itemAmounts.Add(GetNode<Label>("ShieldAmount"));
		itemAmounts.Add(GetNode<Label>("ReactorAmount"));
		itemAmounts.Add(GetNode<Label>("OilAmount"));
		itemAmounts.Add(GetNode<Label>("HologramAmount"));

		itemSlots[0].Frame = 0;
		itemSlots[1].Frame = 0;
		itemSlots[2].Frame = 0;
		itemSlots[3].Frame = 0;
		itemSlots[4].Frame = 0;

		menuTick = GetNode<AudioStreamPlayer>("MenuTick");

		itemSelectors[0].Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		InputHandling();
		scoreLabel.Text = GameData.Instance.GetScore().ToString() + "m";
		float health = GameData.Instance.GetHealth();
		HandleClearedSlot();
		if(GameData.Instance.GetSelectedGadget() != cur) SwitchSelectedItem();
		DisplayHealth(health);
		DisplayItemAmounts();
	}

	private void HandleNewData() {
		if(prev == 0) {
			int gadget = -1;
			for(int i = 0; i < 5; i++) {
				uint amt = GameData.Instance.GetInvetoryAmount(i);
				if(amt > 0) gadget = i;
			}
			if(gadget >= 0) cur = gadget;
		}
	}

	private void InputHandling() {
		if(Input.IsActionJustPressed("select")) {
			if(GameData.Instance.GetCanPlaySFX() && GameData.Instance.GetNumberOfFilledSlots() != 1) menuTick.Play();
			SwitchSelectedItem();
		}
	}

	private void SwitchSelectedItem() {
		ToggleItemSelect(cur);
		int savedItem = GameData.Instance.GetSelectedGadget();
		if(savedItem == cur) FindNextOpenSlot();
		else cur = savedItem;
		ToggleItemSelect(cur);
		GameData.Instance.SetSelectedGadget(cur);
	}

	private void ToggleItemSelect(int item) {
		itemSelectors[item].Visible = !itemSelectors[item].Visible;
	}

	private void FindNextOpenSlot() {
		int start = cur;
		cur = (cur + 1) % 5;
		while(!GameData.Instance.CanUseItem(cur) && cur != start) {
			cur = (cur + 1) % 5;
		}
		if(cur == start && !GameData.Instance.CanUseItem(cur)) cur = (cur + 1) % 5;
	}

	private void HandleClearedSlot() {
		if(!GameData.Instance.CanUseItem(cur) && GameData.Instance.GetTotalItems() > 0) {
			ToggleItemSelect(cur);
			FindNextOpenSlot();
			GameData.Instance.SetSelectedGadget(cur);
			ToggleItemSelect(cur);
		}
	}

	private void DisplayHealth(float health) {
		if(health == 100f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "full";
		}
		else if(health > 82.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "two-thirds";
		}
		else if(health > 75f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "one-third";
		}
		else if(health > 62.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "full";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 50f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "two-thirds";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 37.5f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "one-third";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 25f) {
			healthAnimation0.Animation = "full";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 12.5f) {
			healthAnimation0.Animation = "two-thirds";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else if(health > 0f) {
			healthAnimation0.Animation = "one-third";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
		else {
			healthAnimation0.Animation = "empty";
			healthAnimation1.Animation = "empty";
			healthAnimation2.Animation = "empty";
		}
	}

	private void DisplayItemAmounts() {
		uint amount;
		for(int i = 0; i < 5; i++) {
			amount = GameData.Instance.GetInvetoryAmount(i);
			itemAmounts[i].Text = amount.ToString();
			itemAmounts[i].Visible = amount > 1;
			itemSlots[i].Frame = amount > 0 ? 1 : 0;
		}
	}

	private Label scoreLabel;
	private AnimatedSprite2D healthAnimation0;
	private AnimatedSprite2D healthAnimation1;
	private AnimatedSprite2D healthAnimation2;
	private List<AnimatedSprite2D> itemSlots;
	private List<AnimatedSprite2D> itemSelectors;
	private List<Label> itemAmounts;
	private int prev = 0;
	private int cur = 0;
	private AudioStreamPlayer menuTick;
}

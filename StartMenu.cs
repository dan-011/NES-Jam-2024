using General;
using Godot;
using System;
using System.Collections.Generic;

public partial class StartMenu : CanvasLayer
{
	[Signal]
	public delegate void StartMainMenuEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		generalMenu = GetNode<GeneralMenu>("GeneralMenu");
		controlsMenu = GetNode<ControlsMenu>("ControlsMenu");
		gadgetsGuideMenu = GetNode<GadgetsGuideMenu>("GadgetsGuideMenu");
		settingsMenu = GetNode<SettingsMenu>("SettingsMenu");

		selectOptions = new List<VoidMethod>();
		selectOptions.Add(Resume);
		selectOptions.Add(Controls);
		selectOptions.Add(GadgetsGuide);
		selectOptions.Add(Settings);
		selectOptions.Add(MainMenu);
		selectOptions.Add(ExitGame);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Open() {
		GameData.Instance.PauseAction();
		Visible = true;
		generalMenu.Open();
	}	

	private void OnGeneralMenuSelect(int selectedIndex)
	{
		selectOptions[selectedIndex]();
	}
	
		
	private void OnReturnFromControls()
	{
		controlsMenu.Visible = false;
		generalMenu.Open();
	}

	private void OnReturnFromGadgetsGuide()
	{
		gadgetsGuideMenu.Visible = false;
		generalMenu.Open();
	}

	private void OnReturnFromSettings()
	{
		settingsMenu.Visible = false;
		generalMenu.Open();
	}

	private void Resume() {
		Input.ActionRelease(GameData.Instance.GetA());
		Visible = false;
		generalMenu.SetSelectPosition(0);
		generalMenu.Visible = false;
		GameData.Instance.PlayAction();
		GetTree().Paused = false;
	}

	private void Controls() {
		generalMenu.Visible = false;
		controlsMenu.Open();
	}

	private void GadgetsGuide() {
		generalMenu.Visible = false;
		gadgetsGuideMenu.Open();
	}

	private void Settings() {
		generalMenu.Visible = false;
		settingsMenu.Open();
	}

	private void MainMenu() {
		Resume();
		EmitSignal(SignalName.StartMainMenu);
	}

	private void ExitGame() {
		GetTree().Quit();
	}

	private GeneralMenu generalMenu;
	private ControlsMenu controlsMenu;
	private GadgetsGuideMenu gadgetsGuideMenu;
	private SettingsMenu settingsMenu;
	private delegate void VoidMethod();
	private List<VoidMethod> selectOptions;
}

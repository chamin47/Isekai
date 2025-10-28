using System;

public enum State
{
	Die,
	Moving,
	Idle,
	Skill,
}

public enum Scene
{
	Unknown,
	TitleScene,
	IntroScene,
	LibraryScene,
	GameScene,
	EndingScene,
	RealGameScene,
	LoadingScene,
	DongMinTestScene,
	TestScene,
	TrueEnding_NoRouteScene,
}

public enum Sound
{
	Bgm, 
	Effect,
	SubEffect, 
	MaxCount,
}

public enum UIEvent
{
	Click,
	Drag,
}

public enum MouseEvent
{
	Press,
	PointerDown,
	PointerUp,
	Click,
}

public enum MiniGameDifficulty
{
    Easy,
    Normal,
    Hard,

    Max
}

public enum WorldType
{
    Vinter,
    Chaumm,
    Gang,
    Pelmanus,

	None
}

public enum EasingType
{
	Linear,
	InCubic,
	OutCubic,
	InOutCubic
}
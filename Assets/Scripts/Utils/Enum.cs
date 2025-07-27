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
	Bgm, // 배경음악 
	Effect,// 한번만 실행되는 효과음
	SubEffect, // 멈췄다 다시 실행 가능한 효과음
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

	Max
}
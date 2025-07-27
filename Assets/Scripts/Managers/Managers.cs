using UnityEngine;

public class Managers : MonoBehaviour
{
	private static Managers s_instance;
	private static Managers Instance { get { Init(); return s_instance; } }

	#region Contents
	private GameManagerEx _game = new GameManagerEx();
	private HappinessManager _happy = new HappinessManager();
	private WorldManager _world = new WorldManager();
    private DB _db;
    public static GameManagerEx Game { get { return Instance._game; } }
	public static HappinessManager Happy { get { return Instance._happy; } }
	public static WorldManager World { get { return Instance._world; } }
	public static DB DB { get { return Instance._db; } }
    #endregion

    #region Core
    private SceneManagerEx _scene = new SceneManagerEx();
	private SoundManager _sound = new SoundManager();
	private ResourceManager _resource = new ResourceManager();
	private UIManager _ui = new UIManager();

    public static SceneManagerEx Scene { get { return Instance._scene; } }
	public static SoundManager Sound { get { return Instance._sound; } }
	public static ResourceManager Resource { get { return Instance._resource; } }
	public static UIManager UI { get { return Instance._ui; } }
	#endregion

	public static void Init()
	{
		if (s_instance == null)
		{
			GameObject go = GameObject.Find("@Managers");

			if (go == null)
			{
				go = new GameObject { name = "@Managers" };
				go.AddComponent<Managers>();
			}

			DontDestroyOnLoad(go);

			s_instance = go.GetComponent<Managers>();

			s_instance._game.Init();
			s_instance._sound.Init();
			s_instance._happy.Init();
            s_instance._world.Init();

			if(s_instance._db == null)
			{
				// DB 불러오기
				s_instance._db = s_instance._resource.Load<DB>("DB/DB");
            }

            s_instance._db.Init();
        }
	}

	public static void Clear()
	{
		Scene?.Clear();
		Sound?.Clear();
		UI?.Clear();
	}
}

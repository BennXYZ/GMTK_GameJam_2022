using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void GameStart() => DontDestroyOnLoad(Instantiate(Resources.Load("GameManager")));

    static bool startFromMainMenu = false;
    const string StartFromMainMenuPath = "Game/Start From Main Menu";

    //[MenuItem(StartFromMainMenuPath, priority = 1)]
    //private static void Setting()
    //{
    //    startFromMainMenu = !startFromMainMenu;
    //}

    //[MenuItem(StartFromMainMenuPath, true)]
    //private static bool SettingValidate()
    //{
    //    Menu.SetChecked(StartFromMainMenuPath, startFromMainMenu);
    //    return true;
    //}

    public GameStateManager gameStateManager;
    public InputManager inputManager;

    [Space(15)]

    public GameObject tilePrefab;
    public GameObject interactableIndicatorPrefab;

    static GameManager instance;
    public static GameManager Instance { get => instance; }

    private void Awake()
    {
        instance = this;

        gameStateManager = GetComponent<GameStateManager>();
        inputManager = GetComponent<InputManager>();

#if UNITY_EDITOR
        if(startFromMainMenu)
#endif
        {
            //Start From Main Menu
        }
    }
}

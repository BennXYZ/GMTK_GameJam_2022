using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void GameStart() => DontDestroyOnLoad(Instantiate(Resources.Load("GameManager")));

    private void Awake()
    {
    }
}

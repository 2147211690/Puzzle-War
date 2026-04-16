using TTSDK;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string Code { get; private set; }
    public string AnonymousCode { get; private set; }
        
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var force = true;
        TT.Login((code, anonymousCode, isLogin) =>
            {
                Debug.Log($"TestLogin: force:{force},code:{code},anonymousCode:{anonymousCode},isLogin:{isLogin}");
                Code = code;
                AnonymousCode = anonymousCode;
            },
            (msg) => { Debug.Log($"TestLogin: force:{force},{msg}"); }, force);
    }
}
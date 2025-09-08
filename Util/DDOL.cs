using Dont_Be_Humble.Base;
using Dont_Be_Humble.Injection;
using UnityEngine;

public class DDOL : DBHMono {
    private static DDOL _instance;

    [SerializeField]
    private DependencyInjector dependencyInjector;


    public void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            dependencyInjector.StartUp();
        } else if (_instance != this) {
            Destroy(gameObject);
        }
    }
}
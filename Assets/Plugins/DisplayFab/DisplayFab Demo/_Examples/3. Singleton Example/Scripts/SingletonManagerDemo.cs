using UnityEngine;
using System.Collections;

public class SingletonManagerDemo : MonoBehaviour {

    private static SingletonManagerDemo _instance;

    private static object _lock = new object();

    public static SingletonManagerDemo Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (SingletonManagerDemo)FindObjectOfType(typeof(SingletonManagerDemo));

                    if (FindObjectsOfType(typeof(SingletonManagerDemo)).Length > 1)
                    {
                        Debug.LogWarning("[SingletonManagerDemo] :: Something went really wrong " +
                            " - there should never be more than 1 singleton of this type(" + typeof(SingletonManagerDemo).Name + ")" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<SingletonManagerDemo>();
                        singleton.name = "(SingletonManagerDemo) " + typeof(SingletonManagerDemo).ToString();
                        DontDestroyOnLoad(singleton);

                      
                    }

                    
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    [SerializeField]
    private string _testString="Test String";

    public bool testBoolean { get; set; }

    [SerializeField]
    private Color _colorExample;

    public Color colorExample { get { return _colorExample; } set { _colorExample = value; } }
    public string testString { get { return _testString; } set { _testString = value; } }

    [SerializeField]
    private TeamInfo _teamInfo;

    public TeamInfo teamInfo { get { return _teamInfo; } set { _teamInfo = value; } }

    public string GetTestString() { return "Test"; }


    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}

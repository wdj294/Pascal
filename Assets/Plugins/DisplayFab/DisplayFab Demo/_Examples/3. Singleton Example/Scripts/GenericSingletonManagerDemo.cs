
using Techooka.DisplayFab.Singleton;


public class GenericSingletonManagerDemo : Singleton<GenericSingletonManagerDemo>
{
    protected GenericSingletonManagerDemo() { } // this is to guarantee that the current Manager will be always a singleton only as we can't use the constructor

    public string demoVariableString = "DisplayFab Generic Singleton Works!!!";

}
public class IDisposableObject : System.IDisposable
{
    private bool _valid = true;
    public void Dispose()
    {
        _valid = false;
    }

    public static bool IsValid(IDisposableObject refToCheck)
    {
        return refToCheck != null && refToCheck._valid;
    }
    // how to use it :
    // private void Coroutine myCoroutine -->  private void IDisposableObject myCoroutine
    // inside you async method - [void | System.Threading.Tasks.Task | System.Threading.Tasks.Task<resultType> ] nameOfAsyncFunc( IDisposableObject refToCheck , ... any other)
    // check where is needed to be valid (refToCheck != null && refToCheck.valid) 
    // check where is needed to be valid !(refToCheck != null && refToCheck.valid)

}
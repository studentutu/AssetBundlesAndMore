namespace Hidden
{
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Networking;

    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        private UnityWebRequestAsyncOperation asyncOp;
        private System.Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            this.asyncOp = asyncOp;
            asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted { get { return asyncOp.isDone; } }

        public void GetResult() { }

        public void OnCompleted(System.Action continuation)
        {
            this.continuation = continuation;
        }

        private void OnRequestCompleted(AsyncOperation obj)
        {
            continuation();
        }
    }

}
public static class ExtensionMethods
{
    public static Hidden.UnityWebRequestAwaiter GetAwaiter(this UnityEngine.Networking.UnityWebRequestAsyncOperation asyncOp)
    {
        return new Hidden.UnityWebRequestAwaiter(asyncOp);
    }
}
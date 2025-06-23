namespace CsCamChatPractice.Util
{
    public static class CancellationTokenSourceUtil
    {
        public static void ResetToken(ref CancellationTokenSource? cts)
        {
            try { cts?.Cancel(); } catch { }
            cts = new CancellationTokenSource();
        }
    }
}

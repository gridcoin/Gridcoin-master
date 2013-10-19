namespace BitCoinSharp.Threading
{
    /// <summary>
    /// A marker interface to indicate the task copies the context.
    /// </summary>
    internal interface IContextCopyingTask //NET_ONLY
    {
        /// <summary>
        /// Gets and sets the <see cref="IContextCarrier"/> that captures
        /// and restores the context.
        /// </summary>
        IContextCarrier ContextCarrier { get; set; }
    }
}
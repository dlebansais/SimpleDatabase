namespace Database
{
    /// <summary>
    ///     Indicates how is the connection managed when there is no activity.
    /// </summary>
    public enum ConnectionOption
    {
        /// <summary>
        ///     Keep the connection active all the time.
        /// </summary>
        KeepAlive,
        //ReconnectOnTheFly
    }
}

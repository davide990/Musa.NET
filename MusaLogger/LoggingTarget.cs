namespace MusaLogger
{
    public enum LoggingTarget
    {
        /// <summary>
        /// Address the logger to the console
        /// </summary>
        console = 0,
        /// <summary>
        /// Address the logger to a file
        /// </summary>
        file = 1,
        /// <summary>
        /// Address the logger to a remote logger
        /// </summary>
        remote = 2
    }
}
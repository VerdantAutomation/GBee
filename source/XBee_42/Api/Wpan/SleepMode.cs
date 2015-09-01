namespace NETMF.OpenSource.XBee.Api.Wpan
{
    /// <summary>
    /// Value returned by <see cref="WpanAtCmd.SleepMode"/> command.
    /// </summary>
    public enum SleepMode
    {
        /// <summary>
        ///  TODO: Update Comments
        ///     
        /// </summary>
        NoSleep = 0,

        /// <summary>
        ///  TODO: Update Comments
        ///     
        /// </summary>
        PinHibernate = 1,

        /// <summary>
        ///  TODO: Update Comments
        ///     
        /// </summary>
        PinDoze = 2,

        /// <summary>
        ///  TODO: Update Comments
        ///     
        /// </summary>
        CyclicSleepRemote = 4,

        /// <summary>
        ///  TODO: Update Comments
        ///     
        /// </summary>
        CyclicSleepRemoteWithPinWakeUp = 5,
        
        /// <summary>
        /// For backwards compatibility with firmware v1.x6 only. 
        /// Use <see cref="WpanAtCmd.CoordinatorEnable"/> command.
        /// </summary>
        SleepCoordinator = 6
    }
}
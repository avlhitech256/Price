using System.Threading;
using System.Threading.Tasks;

namespace Common.Thread
{
    /// <summary>
    /// Workaround for issue of TPL in .NET 4.0
    /// Current synchronization context may become null
    /// http://stackoverflow.com/questions/4659257/how-can-synchronizationcontext-current-of-the-main-thread-become-null-in-a-windo?rq=1
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/629d5524-c8db-466f-bc27-0ced11b441ba/taskcontinuewith-from-wcf-client-call-has-no-synchronizationcontext?forum=wcf
    /// It affects in "System.InvalidOperationException: The current SynchronizationContext may not be used as a TaskScheduler"
    /// and system silent crash
    /// </summary>
    public class UIContext
    {
        /// <summary>
        /// Should be used instead of TaskScheduler.FromCurrentSynchronizationContext()
        /// </summary>
        public static TaskScheduler Current { get; private set; }

        /// <summary>
        /// Should be initialized on application startup
        /// </summary>
        public static void Initialize()
        {
            if (Current == null)
            {
                if (SynchronizationContext.Current == null)
                {
                    SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                }

                Current = TaskScheduler.FromCurrentSynchronizationContext();
            }
        }
    }
}

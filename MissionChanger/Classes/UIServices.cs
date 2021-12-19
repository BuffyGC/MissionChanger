using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace MissionChanger.Classes
{
    internal class UIServices
    {
        private static bool IsBusy;

        public static void SetBusyState()
        {
            SetBusyState(true);
        }


        private static void SetBusyState(bool busy)
        {
            if (busy != IsBusy)
            {
                IsBusy = busy;
                Mouse.OverrideCursor = busy ? Cursors.Wait : null;

                if (IsBusy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, dispatcherTimer_Tick, System.Windows.Application.Current.Dispatcher);
                }
            }
        }


        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }
    }
}

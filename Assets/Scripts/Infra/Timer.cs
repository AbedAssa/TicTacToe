using System;
using System.Threading.Tasks;

namespace Infra
{
    /// <summary>
    /// Timer class
    /// </summary>
    public class Timer
    {
        private readonly Action onTimeComplete;                         //The action to be called when timer is done.
        public delegate void OnTimerChanged(float passedTimeNormalized);
        private readonly OnTimerChanged onUpdateCallback;               //function reference to be called everytime the timer updated.
        private int timeMilestoneMilliseconds;                          //The timer milestone to be reached.
        private const int DelayAmountMillisecond = 50;                  //Updating time delay period.
        private float passedTime;                                       //The time already passed.
        private bool stopTimer;                                         //Flag to stop updating the timer.
        private bool isTimerRunning;                                    //Flag to indicate if the timer started counting.
        public Timer(int timeInSeconds,Action onComplete,OnTimerChanged onUpdate)
        {
            timeMilestoneMilliseconds = timeInSeconds * 1000;
            onTimeComplete = onComplete;
            onUpdateCallback = onUpdate;
        }
        
        /// <summary>
        /// Start the timer once for each instance, start updating the timer and call onUpdateCallback
        /// for each loop (update), if timer reach the milestone it will trigger the onTimeComplete action.
        /// </summary>
        private async void Start()
        {
            if (isTimerRunning)
            {
                return;
            }

            while (passedTime < timeMilestoneMilliseconds)
            {
                if (stopTimer)
                {
                    isTimerRunning = false;
                    return;
                }
                isTimerRunning = true;
                await Task.Delay(DelayAmountMillisecond);
                passedTime += DelayAmountMillisecond;
                onUpdateCallback?.Invoke(passedTime / timeMilestoneMilliseconds);
            }

            isTimerRunning = false;
            onTimeComplete?.Invoke();
        }

        /// <summary>
        /// Stop the timer, stopping the timer wont trigger the onTimeComplete action.
        /// </summary>
        public void Stop()
        {
            stopTimer = true;
        }

        /// <summary>
        /// Reset the timer and start over, the function can override the old milestone time, or use the old one.
        /// </summary>
        /// <param name="newMilestone">the new milestone to be reached</param>
        public void Restart(int? newMilestone = null)
        {
            if (newMilestone != null)
            {
                timeMilestoneMilliseconds = newMilestone.Value * 1000;
            }
            stopTimer = false;
            passedTime = 0f;
            Start();
        }
    } 
}
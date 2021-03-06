﻿using EmrWorkflow.Run.Implementation;
using EmrWorkflow.Run.Activities;
using System.Collections.Generic;

namespace EmrWorkflow.Run
{
    /// <summary>
    /// Iterator through the job flow's activities
    /// </summary>
    public abstract class EmrActivitiesIteratorBase
    {
        /// <summary>
        /// Flag indicates that an error has occurred
        /// </summary>
        private bool hasError = false;

        public IEnumerable<EmrActivity> GetActivities(EmrActivitiesRunner emrRunner)
        {
            foreach (EmrActivity successFlowActivity in this.GetNormalFlow(emrRunner))
            {
                if (this.hasError)
                {
                    foreach (EmrActivity failedFlowActivity in this.GetFailedFlow(emrRunner))
                    {
                        yield return failedFlowActivity;
                    }

                    yield break;
                }

                yield return successFlowActivity;
            }
        }

        /// <summary>
        /// Notify an iterator that the job failed, so it can switch to
        /// an alternative activities sequence
        /// or terminate the job flow
        /// or ignore it =)
        /// </summary>
        /// <param name="emrRunner">Reference to the emrRunner</param>
        public virtual void NotifyJobFailed(EmrActivitiesRunner emrRunner)
        {
            this.hasError = true;
        }

        /// <summary>
        /// A normal sequence of activities, if no errors
        /// </summary>
        /// <param name="emrRunner">Reference to the emrRunner</param>
        /// <returns>Sequence of good activities</returns>
        protected abstract IEnumerable<EmrActivity> GetNormalFlow(EmrActivitiesRunner emrRunner);

        /// <summary>
        /// A sequence of activities, if an error has occurred
        /// </summary>
        /// <param name="emrRunner">Reference to the emrRunner</param>
        /// <returns>Sequence of bad activities</returns>
        protected virtual IEnumerable<EmrActivity> GetFailedFlow(EmrActivitiesRunner emrRunner)
        {
            yield break;
        }
    }
}

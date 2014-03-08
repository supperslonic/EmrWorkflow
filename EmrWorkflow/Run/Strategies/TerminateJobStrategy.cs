﻿using Amazon.ElasticMapReduce;
using Amazon.ElasticMapReduce.Model;
using Amazon.Runtime;
using EmrWorkflow.RequestBuilders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmrWorkflow.Run.Strategies
{
    /// <summary>
    /// A strategy to terminate an existing EMR Job
    /// </summary>
    public class TerminateJobStrategy : EmrActivityStrategy
    {
        public TerminateJobStrategy(string name)
            : base(name)
        { }

        /// <summary>
        /// Push a request to EMR service to do some job
        /// </summary>
        /// <param name="emrClient">EMR Client to make requests to the Amazon EMR Service</param>
        /// <param name="settings">Settings to replace placeholders</param>
        /// <param name="jobFlowId">Existing jobflow Id, can be null for the new job.</param>
        /// <returns>JobFlow Id, if request failed -> returns null</returns>
        public override async Task<string> PushAsync(IAmazonElasticMapReduce emrClient, IBuilderSettings settings, string jobFlowId)
        {
            SetTerminationProtectionRequest setTerminationProtectionRequest = new SetTerminationProtectionRequest();
            setTerminationProtectionRequest.JobFlowIds = new List<string> { jobFlowId };
            setTerminationProtectionRequest.TerminationProtected = false;
            AmazonWebServiceResponse response = await emrClient.SetTerminationProtectionAsync(setTerminationProtectionRequest);

            TerminateJobFlowsRequest terminateJobRequest = new TerminateJobFlowsRequest();
            terminateJobRequest.JobFlowIds = new List<string> { jobFlowId };
            response = await emrClient.TerminateJobFlowsAsync(terminateJobRequest);

            return this.IsOk(response) ? jobFlowId : null;
        }
    }
}

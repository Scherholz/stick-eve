using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using Amazon;

public class AWSTest : MonoBehaviour
{
    // Start is called before the first frame update
    private Amazon.Runtime.AWSCredentials m_awsCredentials;
    private Amazon.Lambda.AmazonLambdaClient m_lambda;
    private Amazon.Lambda.Model.InvokeResponse res;
    private Amazon.Lambda.Model.InvokeRequest m_request;
    private Amazon.APIGateway.AmazonAPIGatewayClient m_api_gw_client;

    private Task<Amazon.Lambda.Model.InvokeResponse> m_task;
    public class MessageObj
    {
        public string action;
        public string user;
        public MessageObj(string actionIn, string userIn)
        {
            action = actionIn;
            user = userIn;
        }
    }
    struct jsondata
    {
        public string action;
        public string user;
    }
    private MessageObj _testCommand = new MessageObj("ServerStatus", "Lulu");

    void Start()
    {
        var chain = new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain();

        if (chain.TryGetAWSCredentials("Luis Admin", out m_awsCredentials))
        {
            Debug.Log("Success!");
            
            m_lambda = new Amazon.Lambda.AmazonLambdaClient(m_awsCredentials, RegionEndpoint.SAEast1);
            m_api_gw_client = new Amazon.APIGateway.AmazonAPIGatewayClient(m_awsCredentials, RegionEndpoint.SAEast1);

            jsondata my_jsondata;
            my_jsondata.action = "ServerStatus";
            my_jsondata.user = "Lulu";
            var jsonVar = JsonUtility.ToJson(my_jsondata);
            Debug.Log("Json: " + jsonVar.ToString());
            m_request = new Amazon.Lambda.Model.InvokeRequest
            {
                FunctionName = "server-status-1",
                Payload = JsonUtility.ToJson(my_jsondata)
            };
            //InvokeAwsLambda();

            

        }
        else
        {
            Debug.Log("Unable to get credentials!");
        }    
    }

    async void InvokeAwsLambda()
    {
        try
        {
            res = await m_lambda.InvokeAsync(m_request);

            Debug.Log("log result: " + System.Text.Encoding.UTF8.GetString(res.Payload.ToArray()));
        }
        catch (Exception e)
        {
            Debug.Log("Error: " + e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(res!=null && res.HttpStatusCode== System.Net.HttpStatusCode.OK)
        {
            Debug.Log("Res: " + res.ToString());
            res = null;
        }
        
    }
}

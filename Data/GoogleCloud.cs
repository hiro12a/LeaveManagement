using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;

namespace LeaveManagement.Data
{
    public class GoogleCloud
    {
        private readonly GoogleCredential _googleCredential;

        
        public GoogleCloud()
        {
            string projectId = "ksortreeservice-414322"; // Replace with your actual project ID
            string secretId = "GOOGLESECRETSCREDENTIAL"; // The name of your secret in Secret Manager

            // Create the Secret Manager client
            var client = SecretManagerServiceClient.Create();

            // Access the secret version
            var secretName = new SecretVersionName(projectId, secretId, "latest");
            var secretVersion = client.AccessSecretVersion(secretName);
            var secretPayload = secretVersion.Payload.Data.ToStringUtf8();

            // Create GoogleCredential from the secret payload
            _googleCredential = GoogleCredential.FromJson(secretPayload);
        }

        public GoogleCredential GetGoogleCredential()
        {
            return _googleCredential;
        }
    }
}
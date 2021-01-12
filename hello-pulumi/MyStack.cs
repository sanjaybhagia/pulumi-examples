using System.Text.Json;
using Pulumi;
using Pulumi.Azure.Core;
using Pulumi.Azure.Storage;

class MyStack : Stack
{
    public MyStack()
    {
        // Read configurations
        var config = new Config();

        var project = Deployment.Instance.ProjectName;
        var stack = Deployment.Instance.StackName;

        //This will show the information on console when you are using pulumi preview or pulumi up
        Pulumi.Log.Info($"Project:{project}");
        Pulumi.Log.Info($"Stack:{stack}");

        var location = config.Require("location");
        Pulumi.Log.Info($"location:{location}");

        // Fetch Structured Configuration
        var data = config.RequireObject<JsonElement>("data");
        Pulumi.Log.Info($"Active: {data.GetProperty("active")}");
        Pulumi.Log.Info($"Nums[2] number?: {data.GetProperty("nums")[2]}");

        // Create an Azure Resource Group
        var resourcesName = config.Require("resourcesName");
        var resourceGroupName = $"rg-{resourcesName}-{stack}";

        var resourceGroup = new ResourceGroup("resourceGroup", new ResourceGroupArgs()
        {
            Name = resourceGroupName,
            Location = location
        });

        // Create an Azure Storage Account
        var storageAccountName = $"str{resourcesName}{stack}";
        var storageAccount = new Account("storage", new AccountArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Name = storageAccountName,
            AccountReplicationType = "LRS",
            AccountTier = "Standard",
            Tags =  {
                { "Environment", "Dev" }
            }
        });

        // Export the connection string for the storage account
        this.ConnectionString = storageAccount.PrimaryConnectionString;
    }

    [Output]
    public Output<string> ConnectionString { get; set; }
}

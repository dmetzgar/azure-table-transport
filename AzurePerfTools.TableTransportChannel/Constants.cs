
namespace AzurePerfTools.TableTransportChannel
{
    static class ConfigurationConstants
    {
        internal const string DeploymentId = "deploymentId";
        internal const string ConnectionString = "connStr";
        internal const string RoleName = "role";
        internal const string InstanceName = "instance";
        internal const string RequestTable = "{0}Request";
        internal const string ReplyTable = "{0}Reply";
        internal const string DevelopmentStorage = "UseDevelopmentStorage=true";
        internal const string IdleSleep = "idleSleep";
        internal const string ActiveSleep = "activeSleep";
        internal const int IdleSleepMs = 5000;
        internal const int ActiveSleepMs = 250;
    }
}

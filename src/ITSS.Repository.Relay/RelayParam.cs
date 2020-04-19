namespace ITSS.Repository.Relay
{
    public class RelayParam
    {
        ///<value>Name of your namespace </value>
        public string AzureRelayNamespace;

        ///<value>Name of your hybrid connection </value>
        public string AzureRelayConnectionName;

        ///<value>Name of your Shared Access Policies key, which is RootManageSharedAccessKey by default </value>
        public string AzureRelayKeyName;

        ///<value>Primary key of the namespace you saved earlier </value>
        public string AzureRelayKey;
    }
}

namespace Template.Helper.DataProtected
{
    public interface IDataProtected
    {
        string ProtectInformation(string sensitiveData);

        string UnProtectInformantion(string protectedData);
    }
}
namespace LibraryManagement.Core.Interfaces
{
    public interface IPayloadEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}

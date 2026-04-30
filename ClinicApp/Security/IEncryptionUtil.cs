namespace ClinicApp.Security
{
    public interface IEncryptionUtil
    {
        string Encrypt(string plainText);

        bool isValidPassword(string plainText, string cipherText);
    }
}

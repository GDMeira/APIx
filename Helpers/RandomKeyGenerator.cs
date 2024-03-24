namespace APIx.Helpers;

public class RandomKeyGenerator
{
    public static string GenerateRandomKey()
    {
        Random random = new();
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        string key = "";

        for (int i = 0; i < 36; i++)
        {
            if (i == 8 || i == 13 || i == 18 || i == 23)
            {
                key += "-";
                continue;
            }

            key += chars[random.Next(chars.Length)];
        }

        return key;
    }
}
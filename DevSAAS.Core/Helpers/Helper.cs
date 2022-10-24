using System.Text;

namespace DevSAAS.Core.Helpers;

public class Helper
{
    public static string RandomNumbers(int length)
    {
        var rand = new Random();
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < length; i++)
            stringBuilder.Append(rand.Next(0, 9));

        return stringBuilder.ToString();
    }
}
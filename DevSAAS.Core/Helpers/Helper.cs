using System.Text;

namespace DevSAAS.Core.Helpers;

public class Helper
{
    public static string RandomNumbers(int Length)
    {
        var Rand = new Random();
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < Length; i++)
            stringBuilder.Append(Rand.Next(0, 9));

        return stringBuilder.ToString();
    }
}
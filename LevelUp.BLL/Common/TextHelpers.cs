using System.Globalization;
using System.Text.RegularExpressions;
using LevelUp.DAL.Models.Enums;

namespace LevelUp.BLL.Common;

public static class TextHelpers
{
    public static string NormalizeName(string name) =>
        Regex.Replace(name.Trim(), @"\s+", " ");

    public static string ModeText(GameMode mode) => mode == GameMode.Multi ? "ملتي" : "سنجل";

    public static string MoneyText(decimal amount) => amount.ToString("0.##", CultureInfo.InvariantCulture);
}

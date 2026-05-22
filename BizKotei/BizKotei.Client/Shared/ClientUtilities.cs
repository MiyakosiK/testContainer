namespace BizKotei.Client.Shared;
using BizShareLib.Shared;

public class ClientUtilities
{
    // 金額の文字列をカンマ区切りに変換する
    public static string KingakuCommaSeparate(string? kinnuki)
    {
        if (kinnuki.IsNullOrEmpty()) { return "0"; }
        else
        {
            bool isMinusNumber = kinnuki.Contains('-');
            var kinnukiAbsolute = kinnuki.Replace("-", string.Empty);

            const int separateDidits = 3;
            decimal decimalKingaku;

            if (decimal.TryParse(kinnukiAbsolute, out decimalKingaku))
            {
                //decimalに変換できる入力の場合は変換して、longを通して小数点以下を切り捨てる
                var int64Kingaku = decimal.ToInt64(decimalKingaku);
                var integerKingakuString = int64Kingaku.ToString();

                //3桁ごとの文字列に分けてリストに格納
                var kingakuLength = integerKingakuString.Length;
                List<string> SeparateStrings = new();
                for (int i = 0; i < kingakuLength; i += separateDidits)
                {
                    if (i + separateDidits >= kingakuLength)
                    {
                        var firstDedits = integerKingakuString.Substring(0, kingakuLength - i);
                        SeparateStrings.Insert(0, firstDedits);
                    }
                    else
                    {
                        var threeDegits = integerKingakuString.Substring(kingakuLength - i - separateDidits, separateDidits);
                        SeparateStrings.Insert(0, threeDegits);
                    }
                }

                //3桁ごとにカンマで区切った文字列に成型
                string separatedKingaku = string.Empty;
                bool isFirst = true;
                foreach (string str in SeparateStrings)
                {
                    if (isFirst) { isFirst = false; }
                    else { separatedKingaku += ","; }
                    separatedKingaku += str;
                }

                return isMinusNumber ? $"-{separatedKingaku}" : separatedKingaku;

            }
            else
            {
                return "0";
            }

        }
    }

    /// <summary>
    /// 入力フィールドの種類
    /// </summary>
    public enum TypeKb
    {
        ButtonForInput,     //ボタンのみ（業者ガイド、工事ガイドで使用）
        ButtonForFoward,　　//ボタンのみ（仕入転送で使用）
        TextAndButton,      //入力とボタン（費目～品名で使用）
        TextOnly,           //テキストのみ（規格で使用）
        ClearOnly           //クリアボタンのみ（PC版発注入力で使用）
    }
}
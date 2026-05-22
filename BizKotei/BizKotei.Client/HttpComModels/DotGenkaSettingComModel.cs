namespace BizKotei.Client.HttpComModels.DotGenkaSettingComModel;

/// <summary>
/// レスポンス用のメニュー名称オブジェクトリスト
/// </summary>
public class ListMenuMdiNameResponse
{
    public List<MenuMdiNameResponse>? Data { get; set; }
}

/// <summary>
/// レスポンス用のメニュー名称オブジェクト
/// </summary>
public class MenuMdiNameResponse
{
    public string smmMenuID { get; set; } = default!;
    public string smmMenuNo { get; set; } = default!;
    public string smmMenuSubNo { get; set; } = default!;
    public string smmMenuNm { get; set; } = default!;
}

/// <summary>
/// レスポンス用のマスター名称オブジェクトリスト
/// </summary>
public class ListMstNameResponse
{
    public List<MstNameResponse>? Data { get; set; }
}

/// <summary>
/// レスポンス用のマスター名称オブジェクト
/// </summary>
public class MstNameResponse
{
    public string smnTblID { get; set; } = default!;
    public string smnTblItemName { get; set; } = default!;
}

/// <summary>
/// レスポンス用の項目名称オブジェクトリスト
/// </summary>
public class ListMstFldNameResponse
{
    public List<MstFldNameResponse>? Data { get; set; }
}

/// <summary>
/// レスポンス用の項目名称オブジェクト
/// </summary>
public class MstFldNameResponse
{
    public string smfTblID { get; set; } = default!;
    public string smfFldName { get; set; } = default!;
    public string smfItemName { get; set; } = default!;
    public string smfFldType { get; set; } = default!;
    public string smfFldLen { get; set; } = default!;
}

/// <summary>
/// 初期設定クラス
/// </summary>
public class Set_KaiDef
{
    // 品名のコード体系
    public byte skdGeOpHincdType { get; set; } = default!;
}

/// <summary>
/// 端数設定クラス
/// </summary>
public class Set_HasuInfo
{
    // 積上単価小数点
    public byte shiTsumiTanSyoKb { get; set; } = default!;

    // 単価小数点
    public byte shiTanSyoKb { get; set; } = default!;
}
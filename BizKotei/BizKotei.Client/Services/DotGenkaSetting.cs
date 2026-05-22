using BizClientLib.Services.Interfaces;
using BizKotei.Client.HttpComModels.DotGenkaSettingComModel;
using BizKotei.Client.Services.Interfaces;

namespace BizKotei.Client.Services;

public class DotGenkaSetting : IDotGenkaSetting
{
    private static readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(15);
    private DateTime? expiry = null;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    // メニュー名称一覧（Default）
    private Dictionary<(string, string, string), string> menuNames = new()
    {
        { ("300", "120", "999"), "工事登録" },
        { ("300", "110", "999"), "発注者登録" },
        { ("300", "130", "999"), "社員登録" },
        { ("300", "100", "999"), "業者登録" }
    };

    // マスター名称一覧（Default）
    private Dictionary<string, string> mstNames = new()
    {
        { "Mst_KojiBase", "工事" }, 
        //{ "Mst_Chumon", "発注者" }, 
        //{ "Mst_Syain", "社員" }, 
        { "Mst_Gyosya", "業者" }, 
        { "Mst_Level1", "費目" },
        { "Mst_Level2", "工種" }, 
        { "Mst_Level3", "種別" }, 
        { "Mst_HinDaiBun", "分類" },
        { "Mst_Hinsyu", "品種" },
        { "Mst_Hinmei", "品名" }
    };

    // 項目名称一覧（Default）
    private Dictionary<(string, string), (string, string, string)> mstFldNames = new()
    {
        //{ ("Mst_KojiBase", "mkbBasyo1"), ("工事場所","1","8") },
        { ("Mst_Hinmei", "Hinmei2"), ("規格", "1", "40") },
    };

    // 初期設定
    public Set_KaiDef kaiDef { get; private set; } = new();

    // 端数設定
    public Set_HasuInfo hasuInfo { get; private set; } = new();

    // エラーハンドリングや諸々の処理統一のためにappRequestを呼び元から受け取る
    // サービス同士の依存関係を減らしたいところだったが、上記のメリットを優先する
    public async Task LoadSettingAsync(IAppRequest appRequest)
    {
        var current = DateTime.Now;
        if (expiry != null && expiry > current)
        {
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (expiry == null || expiry <= current)
            {
                List<Task> tasks = new();
                var task1 = appRequest.Get<ListMstNameResponse>("api/DotGenkaSetting/get-master-name");
                var smfTblIDs = new string[] { "Mst_KojiBase", "Mst_Hinmei", "Mst_Gyosya", "Mst_Level1", "Mst_Level2", "Mst_Level3" };
                var queryObj = new { smfTblIDs };
                tasks.Add(task1);
                var task2 = appRequest.Get<ListMstFldNameResponse>("api/DotGenkaSetting/get-master-field-name", queryObj);
                tasks.Add(task2);
                //var task3 = appRequest.Get<ListMenuMdiNameResponse>("api/DotGenkaSetting/get-menu-name");
                var task4 = appRequest.Get<Set_KaiDef>("api/DotGenkaSetting/get-SetKaiDef");
                tasks.Add(task4);
                var task5 = appRequest.Get<Set_HasuInfo>("api/DotGenkaSetting/get-SetHasuInfo");
                tasks.Add(task5);
                await Task.WhenAll(tasks);

                var resMaster = await task1;
                if (resMaster?.Data is not null)
                {
                    mstNames = resMaster.Data.ToDictionary(item => item.smnTblID, item => item.smnTblItemName);
                }

                var resField = await task2;
                if (resField?.Data is not null)
                {
                    mstFldNames = resField.Data.ToDictionary(item => (item.smfTblID, item.smfFldName), item => (item.smfItemName, item.smfFldType, item.smfFldLen));
                }

                //var resMenu = await task3;
                //if (resMenu?.Data is not null)
                //{
                //    menuNames = resMenu.Data.ToDictionary(item => (item.smmMenuID, item.smmMenuNo, item.smmMenuSubNo), item => item.smmMenuNm);
                //}

                var resSetKaiDef = await task4;
                if (resSetKaiDef != null)
                {
                    kaiDef = resSetKaiDef;
                }

                var resSetHasuInfo = await task5;
                if (resSetHasuInfo != null)
                {
                    hasuInfo = resSetHasuInfo;
                }

                expiry = DateTime.Now.Add(cacheDuration);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // メニュー名称
    // デフォルト値もしくは取得値が必ず入るので空白値は返らない。
    // 言語仕様上、Keyに一致しないケースを考慮するため、空白を返す処理を記載している
    public string GetMenuMdiName(string menuID, string menuNo, string subNo) => menuNames.TryGetValue((menuID, menuNo, subNo), out var value) ? value : "";

    // マスター名称
    // デフォルト値もしくは取得値が必ず入るので空白値は返らない。
    // 言語仕様上、Keyに一致しないケースを考慮するため、空白を返す処理を記載している
    public string GetMstName(string targetItemName) => mstNames.TryGetValue(targetItemName, out string? value) ? value : "";

    // 項目名称
    // デフォルト値もしくは取得値が必ず入るので空白値は返らない。
    // 言語仕様上、Keyに一致しないケースを考慮するため、空白を返す処理を記載している
    public string GetMstFldName(string tblID, string fldName) => mstFldNames.TryGetValue((tblID, fldName), out var value) ? value.Item1 : "";

    // 項目名称（属性）
    // デフォルト値もしくは取得値が必ず入るので空白値は返らない。
    // 言語仕様上、Keyに一致しないケースを考慮するため、空白を返す処理を記載している
    public string GetMstFldType(string tblID, string fldName) => mstFldNames.TryGetValue((tblID, fldName), out var value) ? value.Item2 : "";

    // 項目名称（桁数）
    // デフォルト値もしくは取得値が必ず入るので空白値は返らない。
    // 言語仕様上、Keyに一致しないケースを考慮するため、空白を返す処理を記載している
    public string GetMstFldLen(string tblID, string fldName) => mstFldNames.TryGetValue((tblID, fldName), out var value) ? value.Item3 : "";

    // 初期設定
    public Set_KaiDef GetKaiDef() => kaiDef ?? new Set_KaiDef();

    // 端数設定
    public Set_HasuInfo GetHasuInfo() => hasuInfo ?? new Set_HasuInfo();
}

using BizKotei.Client.HttpComModels.DotGenkaSettingComModel;
using BizServerLib.Controllers;
using BizServerLib.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BizKotei.Controllers;

public class DotGenkaSettingController(IWebApiRequest webApiRequest) : AppControllerBase
{
    private readonly IWebApiRequest _webApiRequest = webApiRequest;

    // メニュー名称
    [HttpGet("get-menu-name")]
    public async Task<ActionResult<ListMenuMdiNameResponse?>> GetMenuMdiNm()
    {
        var result = await _webApiRequest.Get<ListMenuMdiNameResponse>("api/BizMenuMdiNm/GetMenuMdiNm");
        return result;
    }

    // マスター名称
    [HttpGet("get-master-name")]
    public async Task<ActionResult<ListMstNameResponse?>> GetMstNm()
    {
        var result = await _webApiRequest.Get<ListMstNameResponse>("api/BizMstNm/GetMstNm");
        return result;
    }

    // 項目名称
    [HttpGet("get-master-field-name")]
    public async Task<ActionResult<ListMstFldNameResponse?>> GetMstFldNm([FromQuery] string[] smfTblIDs)
    {
        string[]? fieldList;
        if (smfTblIDs is null || smfTblIDs.Length == 0)
        {
            fieldList = null;
        }
        else
        {
            fieldList = smfTblIDs;
        }
        var queryObj = new { smfTblIDs = fieldList };

        var result = await _webApiRequest.Get<ListMstFldNameResponse>("api/BizMstFldNm/GetMstFldNm", queryObj);
        return result;
    }

    // 初期設定
    [HttpGet("get-SetKaiDef")]
    public async Task<ActionResult<Set_KaiDef?>> GetSetKaiDef()
    {
        return await _webApiRequest.Get<Set_KaiDef>("api/BizSetKaiDef/GetKaiDef", null);
    }

    // 端数設定
    [HttpGet("get-SetHasuInfo")]
    public async Task<ActionResult<Set_HasuInfo?>> GetSetHasuInfo()
    {
        return await _webApiRequest.Get<Set_HasuInfo>("api/BizSetHasuInfo/GetHasuInfo", null);
    }
}

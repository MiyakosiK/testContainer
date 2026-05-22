using BizClientLib.Services.Interfaces;
using BizKotei.Client.HttpComModels.DotGenkaSettingComModel;

namespace BizKotei.Client.Services.Interfaces;

public interface IDotGenkaSetting
{
    public Task LoadSettingAsync(IAppRequest appRequest);

    public string GetMenuMdiName(string menuID, string menuNo, string subNo);

    public string GetMstName(string targetItemName);

    public string GetMstFldName(string tblID, string fldName);

    public string GetMstFldType(string tblID, string fldName);

    public string GetMstFldLen(string tblID, string fldName);

    public Set_KaiDef GetKaiDef();

    public Set_HasuInfo GetHasuInfo();
}

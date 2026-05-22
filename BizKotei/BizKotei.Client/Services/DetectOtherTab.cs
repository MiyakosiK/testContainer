using BizClientLib.Components.Modal;
using BizClientLib.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BizShareLib.HttpComModels.DataConnectorComModel;
using BizClientLib.Shared;

namespace BizKotei.Client.Services;

public class DetectOtherTab : ComponentBase
{
    [Inject]
    IJSRuntime JS { get; set; } = default!;

    [Inject]
    IAppRequest AppRequest { get; set; } = default!;

    protected MessageBox messageBox = default!;

    //アプリ起動時の接続情報を保持
    private static DataConnectForm oldresDCForm = new DataConnectForm();
    private static string oldstorageKey = "";

    //ローカルストレージ（userSignedIn）が変更された時の接続情報を保持
    private DataConnectForm resDCForm = new DataConnectForm();
    private string storageKey = "";

    private DotNetObjectReference<DetectOtherTab>? _objectReference;

    protected override async Task OnInitializedAsync()
    {
        _objectReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("registerSignOutListener", _objectReference);
    }

    [JSInvokable]
    public async Task OnUserSignedIn()
    {
        resDCForm = await AppRequest.Get<DataConnectForm>("api/DataConnector/get-cookie") ?? new DataConnectForm();
        storageKey = ClientUtility.GetBase64EncoedSha256Hash(resDCForm.TenantID + resDCForm.DBName + resDCForm.UserID);

        if (oldstorageKey is not null && storageKey is not null)
        {
            if (oldstorageKey == storageKey)
            {
                //一致している時は何もしない
            }
            else
            {
                await messageBox.InfoAsync("他タブでサインアウトされました。<br />ページを再読み込みします。");
                await JS.InvokeVoidAsync("reloadPage");
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //全ての画面で継承しているためここに記載（MainLayoutでは動作せず）
            await JS.InvokeVoidAsync("goBackToHome");
            if (oldresDCForm.TenantID is null)
            {
                oldresDCForm = await AppRequest.Get<DataConnectForm>("api/DataConnector/get-cookie") ?? new DataConnectForm();
                oldstorageKey = ClientUtility.GetBase64EncoedSha256Hash(oldresDCForm.TenantID + oldresDCForm.DBName + oldresDCForm.UserID);
            }
        }
    }
}
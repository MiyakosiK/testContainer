using BizShareLib.Shared;

namespace BizKotei.Client.Shared
{
    public class PathInfo
    {
        /// <summary>
        /// Path 一覧
        /// </summary>
        public class Path
        {
            public const string Home = "/";
            public const string Setting = "/setting";
            public const string Help = "/help";
            public const string Account = "/account";
            public const string DataConnector = "/data-connector";
        }

        /// <summary>
        /// IconとTitleのペアモデル
        /// </summary>
        public class IconTitlePair
        {
            public string Title { get; set; } = string.Empty;
            public string IconSrc { get; set; } = "/images/breadcrumbs-chevron.png";
            public string MenuIconSrc { get; set; } = "/images/breadcrumbs-chevron.png";
        }

        // Home系のPathとTitleとIcon
        public static Dictionary<string, IconTitlePair> GetHomeGroupPathTitle()
        {
            return new Dictionary<string, IconTitlePair>()
            {
                {Path.Home, new IconTitlePair{ Title ="ホーム" , IconSrc="/images/home_blue.png" , MenuIconSrc="/images/home_black.png" } }
            };
        }

        // Home系以外のPathとTitle
        public static Dictionary<string, IconTitlePair> GetPathTitle()
        {
            return new Dictionary<string, IconTitlePair>()
            {
                {Path.Setting,new IconTitlePair{ Title = "設定"} },

                // 本来、パンくずリストに表示されないPathではあるが、
                // 401、403エラー時のリダイレクトとパンくずリストの生成タイミング(OnInitialized)で何等かの異常がおこり、
                // Dictionary検索でエラーが発生するので追加する。
                {CommonPathInfo.Path.SsoRedirect,new IconTitlePair { Title = "",IconSrc="" }},
                {CommonPathInfo.Path.DataConnector,new IconTitlePair { Title = "",IconSrc="" }}
            };
        }
    }
}

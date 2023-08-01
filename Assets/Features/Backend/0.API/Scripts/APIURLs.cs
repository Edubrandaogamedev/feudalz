public class APIURLs
{
    public class DataBaseUrls
    {
#if DEVELOP
        public const string DATABASE_URL = "https://dev.game-api.feudalz.io";
        //public const string DATABASE_URL = "https://dev.game-server.feudalz.io";
        //public const string DATABASE_URL = "https://feud1.herokuapp.com";
#else
        //public const string DATABASE_URL = "https://feudalz-game-server-djaxojxmla-uc.a.run.app";
        public const string DATABASE_URL = "https://game-server.feudalz.io";
#endif
        public const string NONCE_CONTEXT = "/nonce";
        public const string LOGIN_CONTEXT = "/login";
        public const string INFO_CONTEXT = "/info";
        public const string INFO_FAST_CONTEXT = "/fastInfo";
        public const string ATTACK_CONTEXT = "/attack";
        public const string RECHARGE_ALL_CONTEXT = "/rechargeAll";
        public const string CHECK_RECHARGE_ALL_COST_CONTEXT = "/checkRechargeAll";
        public const string ATTACK_ALL_CONTEXT = "/attackAll";
        public const string BATTLE_LOG_CONTEXT = "/history";
        public const string RANKING_LOG_CONTEXT = "/ranking";
        public const string FIXLANDZ_CONTEXT = "/healLand";
        public const string FIXLANDZ_ALL_CONTEXT = "/healAllLandz";
        public const string INSTALL_HERO = "/installHero";
        public const string REMOVE_HERO = "/removeHero";
        public const string ALLOCATE_BUILD = "/allocateBuilding";
        public const string REMOVE_ALLOCATED_BUILD = "/removeBuilding";
        public const string START_PRODUCTION = "/startProd";
        public const string CANCEL_PRODUCTION = "/cancelBatch";
        public const string HARVEST = "/harvest";
        public const string HARVEST_ALL = "/harvestAll";
        public const string AUTO_REBUILD_CONTEXT = "/switchAutoRebuild";
        public const string AUTO_CRAFT_CONTEXT = "/switchAutoCraft";

    }
}

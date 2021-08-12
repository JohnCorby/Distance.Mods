namespace Distance.ML {
    public static class Utils {
        public static PlayerDataLocal? playerDataLocal => G.Sys.PlayerManager_.Current_?.playerData_;
    }
}

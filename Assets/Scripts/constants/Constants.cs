using UnityEngine.SceneManagement;

public static class Constants
{
    // scene name constants
    public const string SCENE_NAME_level_00_account = "level_00_account";
    public const string SCENE_NAME_level_00_account_createNew = "level_00_account_createNew";
    public const string SCENE_NAME_level_00_account_loginExisting = "level_00_account_loginExisting";
    public const string SCENE_NAME_level_00_account_loginLocal = "level_00_account_loginLocal";
    public const string SCENE_NAME_level_00_credits = "level_00_credits";
    public const string SCENE_NAME_level_00_loading = "level_00_loading";
    public const string SCENE_NAME_level_00_options = "level_00_options";
    public const string SCENE_NAME_level_00_progression = "level_00_progression";
    public const string SCENE_NAME_level_00_start = "level_00_start";
    public const string SCENE_NAME_level_00_stats = "level_00_stats";
    public const string SCENE_NAME_level_01_scrapyard = "level_01_scrapyard";
    public const string SCENE_NAME_level_02_circlek = "level_02_circlek";
    public const string SCENE_NAME_level_03_snow = "level_03_snow";
    public const string SCENE_NAME_level_04_slab = "level_04_slab";
    public const string SCENE_NAME_level_05_aveb = "level_05_aveb";
    public const string SCENE_NAME_level_06_caffe = "level_06_caffe";
    public const string SCENE_NAME_level_07_sudan = "level_07_sudan";
    public const string SCENE_NAME_level_08_tammys = "level_08_tammys";
    public const string SCENE_NAME_level_09_party_mansion = "level_09_party_mansion";
    public const string SCENE_NAME_level_10_time_jail = "level_10_time_jail";
    public const string SCENE_NAME_level_11_forest = "level_11_forest";
    public const string SCENE_NAME_level_12_theater = "level_12_theater";
    public const string SCENE_NAME_level_13_rustys = "level_13_rustys";
    public const string SCENE_NAME_level_14_dome = "level_14_dome";
    public const string SCENE_NAME_level_15_cocaine_island = "level_15_cocaine_island";
    public const string SCENE_NAME_level_16_boner_mountain = "level_16_boner_mountain";
    public const string SCENE_NAME_level_17_steel_cage = "level_17_steel_cage";

    // dev server api address constants
    public const string API_ADDRESS_DEV_publicApi = "http://skeletondistrict.com/api/";
    public const string API_ADDRESS_DEV_publicApiUsers = "http://skeletondistrict.com/api/users";
    public const string API_ADDRESS_DEV_publicApiUsersByUserid = "http://skeletondistrict.com/api/users/userid";
    public const string API_ADDRESS_DEV_publicApiUsersByUserName = "http://skeletondistrict.com/api/users/username/";
    public const string API_ADDRESS_DEV_publicApiUsersByEmail = "http://skeletondistrict.com/api/users/email/";
    public const string API_ADDRESS_DEV_publicApiHighScores = "http://skeletondistrict.com/api/highscores/";
    public const string API_ADDRESS_DEV_publicApiHighScoresByScoreid = "http://skeletondistrict.com/api/highscores/scoreid/";
    public const string API_ADDRESS_DEV_publicApiHighScoresByModeid = "http://skeletondistrict.com/api/highscores/modeid/";
    public const string API_ADDRESS_DEV_publicApiHighScoresCountByModeid = "http://skeletondistrict.com/api/highscores/modeid/count/";
    public const string API_ADDRESS_DEV_publicApiHighScoresByModeidInGameDisplayAll = "http://skeletondistrict.com/api/highscores/modeid/all/";
    public const string API_ADDRESS_DEV_publicApiHighScoresByModeidInGameDisplayFiltered = "http://skeletondistrict.com/api/highscores/modeid/filter/";
    public const string API_ADDRESS_DEV_publicApiHighScoresByPlatform = "http://skeletondistrict.com/api/highscores/platform/";
    public const string API_ADDRESS_DEV_publicApiToken = "http://skeletondistrict.com/api/token/";
    public const string API_ADDRESS_DEV_publicApplicationVersionCurrent = "http://skeletondistrict.com/api/application/version/current";
    public const string API_ADDRESS_DEV_publicUserReport = "http://skeletondistrict.com/api/userreport";

    // localhost testing
    public const string API_ADDRESS_LOCALHOST_HighScoresByModeidInGameDisplay = "https://localhost:44362/api/highscores/game/modeid/";
    public const string API_ADDRESS_LOCALHOST_HighScoresCountByModeid = "https://localhost:44362/api/highscores/modeid/count/";

    //sqlite Database tables
    public const string LOCAL_DATABASE_tableName_allTimeStats = "AllTimeStats";
    public const string LOCAL_DATABASE_tableName_characterProfile = "CharacterProfile";
    public const string LOCAL_DATABASE_tableName_cheerleaderProfile = "CheerleaderProfile";
    public const string LOCAL_DATABASE_tableName_highscores = "HighScores";
    public const string LOCAL_DATABASE_tableName_user = "User";

}

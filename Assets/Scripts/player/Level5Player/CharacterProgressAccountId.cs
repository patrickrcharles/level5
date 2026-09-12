using Level5.Core;

public static class CharacterProgressAccountId
{
    public static string GetCurrent()
    {
        if (LocalAccountIdentity.UserId > 0)
        {
            return LocalAccountIdentity.UserId.ToString();
        }

        if (!string.IsNullOrWhiteSpace(LocalAccountIdentity.UserName))
        {
            return LocalAccountIdentity.UserName;
        }

        return "guest";
    }
}

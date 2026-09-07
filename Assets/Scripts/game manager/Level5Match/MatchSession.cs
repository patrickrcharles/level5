using System;

public static class MatchSession
{
    private static string currentResultId;

    public static string BeginNewMatch()
    {
        currentResultId = CreateResultId("match");
        return currentResultId;
    }

    public static string EnsureCurrentMatch()
    {
        return string.IsNullOrEmpty(currentResultId)
            ? BeginNewMatch()
            : currentResultId;
    }

    public static string CreateResultId(string prefix)
    {
        string safePrefix = string.IsNullOrEmpty(prefix) ? "match" : prefix;
        return safePrefix + "-" + Guid.NewGuid().ToString("N");
    }
}

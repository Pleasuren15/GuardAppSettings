namespace GuardAppSettings.Tests;

public static class Helpers
{
    public static string GetSolutionDirectory()
    {
        var startPath = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(startPath);

        while (directory != null)
        {
            if (directory.GetFiles("*.sln").Length > 0)
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Solution file not found in the directory hierarchy.", startPath);
    }
}

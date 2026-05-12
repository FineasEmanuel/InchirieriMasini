using System.IO;

namespace Inchirieri.Data.Stocare
{
    public static class DataFiles
    {
        public static string GetPath(string fileName)
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current != null)
            {
                bool isRepositoryRoot =
                    File.Exists(Path.Combine(current.FullName, "Inchirieri.sln")) ||
                    File.Exists(Path.Combine(current.FullName, "Inchirieri.slnx"));

                if (isRepositoryRoot)
                {
                    return EnsureDataPath(current.FullName, fileName);
                }

                current = current.Parent;
            }

            return EnsureDataPath(AppContext.BaseDirectory, fileName);
        }

        private static string EnsureDataPath(string root, string fileName)
        {
            string dataDirectory = Path.Combine(root, "data");
            Directory.CreateDirectory(dataDirectory);
            return Path.Combine(dataDirectory, fileName);
        }
    }
}

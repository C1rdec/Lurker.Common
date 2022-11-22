using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FuzzySharp;

namespace Lurker.Common.Models
{
    public abstract class GameBase
    {
        #region Properties

        public string ExeFilePath { get; set; }

        public string Name { get; set; }

        public abstract LauncherType Launcher { get; }

        public abstract string Id { get; }

        public virtual Dictionary<string, string> Alias => new();

        #endregion

        #region Methods

        public abstract Task Open();

        public Bitmap GetIcon()
        {
            if (string.IsNullOrEmpty(ExeFilePath))
            {
                return null;
            }

            var result = (Icon)null;

            try
            {
                result = Icon.ExtractAssociatedIcon(ExeFilePath);
            }
            catch (System.Exception)
            {
                // Set Default Icon
            }

            return result.ToBitmap();
        }

        public abstract void Initialize();

        protected void SetExeFile(string installationFolder)
        {
            var exeFiles = new DirectoryInfo(installationFolder).GetFiles($"*.exe", SearchOption.AllDirectories);
            if (exeFiles.Any())
            {
                var searchTerm = Name;
                if (Alias.TryGetValue(Id, out var alias))
                {
                    searchTerm = alias;
                }

                var matches = exeFiles.Select(e => new
                {
                    FilePath = e.FullName,
                    Ratio = Fuzz.Ratio(e.Name.Replace(".exe", string.Empty).ToLower(), searchTerm.ToLower())
                });

                var bestmatch = matches.MaxBy(r => r.Ratio);
                ExeFilePath = bestmatch.FilePath;
            }
        }

        #endregion
    }
}

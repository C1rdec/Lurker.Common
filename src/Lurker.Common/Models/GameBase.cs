using System.Drawing;
using System.Threading.Tasks;

namespace Lurker.Common.Models
{
    public abstract class GameBase
    {
        #region Properties

        public string ExeFilePath { get; set; }

        public string Name { get; set; }

        public abstract LauncherType Launcher { get; }

        public abstract string Id { get; }

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

        #endregion
    }
}

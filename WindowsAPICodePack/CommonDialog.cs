using Microsoft.Win32;
using System;

namespace V3X.WindowsAPICodePack.Dialogs
{
    public enum CommonFileDialogResult
    {
        /// <summary>
        /// Default value for enumeration, a dialog box should never return this value.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dialog box return value is OK (usually sent from a button labeled OK or Save).
        /// </summary>
        Ok = 1,

        /// <summary>
        /// The dialog box return value is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel = 2,
    }

    public class CommonSaveFileDialog : IDisposable
    {
        public bool IsFolderPicker { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }

        private SaveFileDialog dialog = new SaveFileDialog();
        

        public CommonFileDialogResult ShowDialog()
        {
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = Filter;
          

            var ret = dialog.ShowDialog();
            if (ret == true)
            {
                FileName = dialog.FileName;
                if (IsFolderPicker)
                {
                    FileName = System.IO.Path.GetDirectoryName(FileName);
                }
                return CommonFileDialogResult.Ok;
            }
            else
            {
                return CommonFileDialogResult.Cancel;
            }
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CommonOpenFileDialog() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class CommonOpenFileDialog : IDisposable
    {
        public bool IsFolderPicker { get; set; }
        public string FileName { get; set; }
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }

        private OpenFileDialog dialog = new OpenFileDialog();


        public CommonFileDialogResult ShowDialog()
        {
            dialog.InitialDirectory = InitialDirectory;
            dialog.FileName = FileName;
            dialog.Filter = Filter;

            if (IsFolderPicker)
            {
                dialog.ValidateNames = false;
                dialog.CheckFileExists = false;
                dialog.CheckFileExists = true;
            }

            var ret = dialog.ShowDialog();
            if (ret == true)
            {
                FileName = dialog.FileName;
                if (IsFolderPicker)
                {
                    FileName = System.IO.Path.GetDirectoryName(FileName);
                }
                return CommonFileDialogResult.Ok;
            }
            else
            {
                return CommonFileDialogResult.Cancel;
            }

        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CommonOpenFileDialog() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

   

}

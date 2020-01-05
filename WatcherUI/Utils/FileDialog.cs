using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherUI.Utils
{
    public static class FileDialog
    {
        public static string GetFilePath(string extensionFilter, string oldFilePath = null)
        {
            string path = oldFilePath;

            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = extensionFilter };

            if (!string.IsNullOrEmpty(oldFilePath))
            {
                openFileDialog.FileName = oldFilePath;
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(oldFilePath);
            }

            if(openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;

            return path;
        }
    }
}

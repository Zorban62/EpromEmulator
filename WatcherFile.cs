using System;
using System.Media;
using System.IO;
using System.Windows.Forms;


//WatcherFile WF = new WatcherFile(this, File_Path_Hex, LoadFileHex);

namespace Andrea_NameSpace
{
    class WatcherFile
    {
        FileSystemWatcher fswHex;
        private string File_Path;
        
        public delegate void ReLoadFile(string File);
        private ReLoadFile Reload_File;
        
        public WatcherFile(object obj, string F_Path, ReLoadFile RLF)
        {
            Reload_File = RLF;
            File_Path = F_Path;
            fswHex = new FileSystemWatcher();
            fswHex.Path = Path.GetDirectoryName(File_Path);
            fswHex.SynchronizingObject = (System.ComponentModel.ISynchronizeInvoke)obj; //Sincronizza il watcher con questo oggetto (thread oggetto)
            fswHex.NotifyFilter = NotifyFilters.LastWrite;
            fswHex.Filter = Path.GetFileName(File_Path);
            fswHex.Changed += new FileSystemEventHandler(HexChange);
            fswHex.EnableRaisingEvents = true; //Abilita il controllo LastWrite 
        }

        private void HexChange(object source, FileSystemEventArgs e)
        {
            fswHex.EnableRaisingEvents = false; //Blocca il secondo evento di FileChange (seconda scrittura per modificare gli attributi)
            SystemSounds.Beep.Play();
            //monitor.ClearMonitor();
            if (File.Exists(File_Path))
            {
                //monitor.ScriviSuMonitor("File Modificato ultima scrittura {0}, ricarico file", File.GetLastWriteTime(Nome_File).ToString("HH:mm:ss:FFF"));
                //monitor.ScriviSuMonitor("File {0} Modificato ultimo accesso {1}", File_Path_Hex, File.GetLastAccessTime(File_Path_Hex).ToString("HH:mm:ss:FFF"));

                DialogResult result = MessageBox.Show("File " + " modificato vuoi ricaricarlo?", "File Change", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.Yes) Reload_File(File_Path);
                fswHex.EnableRaisingEvents = true; //Ri-Abilita il controllo LastWrite
            }
            //else monitor.ScriviSuMonitor("Il file {0} non esiste più", File_Path_Hex);
        }

    }
}

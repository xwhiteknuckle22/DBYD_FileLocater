using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBYD_FileLocater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //refreshButton_Click(null, null);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private List<string> getFilenames(string match)
        {
            // Ensure that the input match isn't empty or null.
            if (string.IsNullOrEmpty(match) || string.IsNullOrWhiteSpace(match))
            {
                return new List<string>();
            }
            
            // Set a variable to the current users root profile directory and begin search from there.
            string root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // To hold the final list of strings.
            var files = new List<string>();

            // Get the fully qualified path name
            var rootDirectory = new DirectoryInfo(root);
            root = rootDirectory.FullName;

            // Variables for the while loop to ensure that we skip folders for which this program doesn't have permission.
            var folders = new Queue<string>();
            folders.Enqueue(root);

            // Iterate through the list of folders beginning with root and find all of the files that match the specified file name criteria.
            while (folders.Count != 0)
            {
                string currentFolder = folders.Dequeue();

                // Get all of the matching files from the current directory
                try
                {
                    // TODO: change this string to match the project file names.....
                    var currentFiles = Directory.EnumerateFiles(currentFolder, match);
                    files.AddRange(currentFiles);
                }
                // Ignore these exceptions
                catch (UnauthorizedAccessException uae)
                {
                    //Console.WriteLine(uae.Message);
                }
                catch (PathTooLongException ptle)
                {
                    //Console.WriteLine(ptle.Message);
                }
                catch (IOException ioe)
                {
                    //DO Something...
                }

                // Attempt to open all of the subdirectories from the current one. Ignore errors.
                try
                {
                    var currentSubFolders = Directory.GetDirectories(currentFolder);
                    foreach (string current in currentSubFolders)
                    {
                        folders.Enqueue(current);
                    }
                }
                // Ignore these exceptions
                catch (UnauthorizedAccessException uae)
                {
                    //Console.WriteLine(uae.Message);
                }
                catch (PathTooLongException ptle)
                {
                    //Console.WriteLine(ptle.Message);
                }
                catch (IOException ioe)
                {
                    // Do Shit....
                }
            }

            return files;
        }

        private void dbydListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the currently selected item in the dbyd ListBox.
            string curItem = dbydListBox.SelectedItem.ToString();

            // Clear the related files listbox
            relatedFilesListBox.Items.Clear();                

            // Create the string that we want to match files to from the master string
            var match = Path.GetFileNameWithoutExtension(curItem);

            var files = getFilenames(match + ".txt");

            if (files.Count() < 1)
            {
                relatedFilesListBox.Items.Add("<None Found>");
            }
            else
            {
                // Display the previously created list of files in the listview.
                foreach (var f in files)
                {
                    relatedFilesListBox.Items.Add(f);
                }
            }

            if (!string.IsNullOrEmpty(match) && !string.IsNullOrWhiteSpace(match))
            {
                // Find all related file names

                // Display those files
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            // Make the mouse a spinning wheel to indicate processing in progress.
            //Application.UseWaitCursor = false;

            // Start by clearing the listview to avoid duplicate entry's.
            dbydListBox.Items.Clear();

            var inputMatch = dbydFilenameInput.Text;
            if (string.IsNullOrEmpty(inputMatch) || string.IsNullOrWhiteSpace(inputMatch))
            {
                dbydListBox.Items.Add("<None Found>");
                return;
            }

            //var files = getFilenames("*test*.pdf");
            var files = getFilenames(inputMatch);

            if (files.Count() < 1)
            {
                dbydListBox.Items.Add("<None Found>");
            }
            else
            {
                // Display the previously created list of files in the listview.
                foreach (var f in files)
                {
                    dbydListBox.Items.Add(f);
                }
            }
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            // Get the currently selected item in the dbyd ListBox.
            string curItem = dbydListBox.SelectedItem.ToString();

            // Create the directory for the files it it doesn't already exist.
            var newDir = Path.ChangeExtension(curItem, null);
            newDir = newDir + "_responses";
            try
            {
                if (!Directory.Exists(newDir))
                {
                    DirectoryInfo di = Directory.CreateDirectory(newDir);
                }
            }
            catch (Exception)
            {
                // TODO: print an error message...
            }            

            // Move all of the files from the related files listbox into the new directory.
            foreach (var f in relatedFilesListBox.Items)
            {
                try
                {
                    File.Move(f.ToString(), newDir + "\\" + Path.GetFileName(f.ToString()));
                }
                catch (Exception)
                {
                    // TODO: print an error message...
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Configure message box
            string message = "Created by Adam Unger\nDBYD File Locater v1.0\nEmail: adamunger0@gmail.com";
            // Show message box
            var result = MessageBox.Show(message);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            relatedFilesListBox.Items.Remove(relatedFilesListBox.SelectedItem);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
            if (selectFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fNames = selectFileDialog.FileNames;
                foreach (var fName in fNames)
                {
                    if (!string.IsNullOrEmpty(fName) && !string.IsNullOrWhiteSpace(fName))
                    {
                        relatedFilesListBox.Items.Add(fName);
                        return;
                    }
                }
            }
        }
    }
}

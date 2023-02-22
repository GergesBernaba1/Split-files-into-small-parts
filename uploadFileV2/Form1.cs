using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace uploadFileV2
{
    public partial class Form1 : Form
    {
        static string filename;
        public byte[] FileZIPArray;
        string SaveFileFolder = @"D:/Images/VIP";
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = "C://Desktop";
            openFileDialog1.Title = "Select file to be upload.";

            openFileDialog1.FilterIndex = 1;
            try
            {

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //check size file less than 100 MB
                    var fileInfo = new FileInfo(openFileDialog1.FileName);
                    if (fileInfo.Length > 9.9e+7)
                    {
                        MessageBox.Show("you file greater than 99 MB");
                        label2.Text = null;
                    }
                    if (openFileDialog1.CheckFileExists)
                    {
                        string path = System.IO.Path.GetFullPath(openFileDialog1.FileName);
                        filename = fileInfo.Name;
                        label2.Text = path;
                    }
                }
                else
                {
                    MessageBox.Show("Please Upload file.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            //using (FileStream csvStream = File.Open(label2.Text, FileMode.Open, FileAccess.Read))
            //{
            //    using (MemoryStream zipToCreate = new MemoryStream())
            //    {
            //        using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create, true))
            //        {
            //            ZipArchiveEntry fileEntry = archive.CreateEntry(Path.GetFileName(label2.Text));
            //            using (var entryStream = fileEntry.Open())
            //            {
            //                csvStream.CopyTo(entryStream);
            //            }

            //        }

            //        FileZIPArray = zipToCreate.ToArray();
            //    }
            //}


            string filepath = label2.Text;
            Array FileZIPArray = File.ReadAllBytes(filepath);
            string Part = "Part";

            int partSize = FileZIPArray.Length / 3;

            int Count = 0;
            for (int i = 0, j = partSize; j <= FileZIPArray.Length; i = j, j += partSize)
            {
                Count++;
                //get remain byte
                if (Count == 3)
                {
                    partSize = FileZIPArray.Length - (partSize * 2);
                }
                byte[] result = new byte[partSize];
                Array.Copy(FileZIPArray, i, result, 0, partSize);
                string partasbase64String = Convert.ToBase64String(result);
                using (var stream = new FileStream($"D:/AfterParts_{filename}", FileMode.Append))
                {
                    stream.Write(result, 0, result.Length);
                }

                ///   Post(partasbase64String, Part + Count + "_" + filename, "https://histocrapi.expertapps.com.sa/api/WsiManager/UploadWsiPart");

            }


        }
        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
      
        public void Post(string attachment, string partName, string uri)
        {
            var body = new { attachment = attachment, partName = partName };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = System.Threading.Timeout.Infinite;
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string AuditDtoJson = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                streamWriter.Write(AuditDtoJson);
                streamWriter.Flush();
            }

            httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    return true;
            //}
        }

       
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("D:/Images");
            ComboBox cb = new ComboBox();
            cb.Location = new Point(10, 22);
            cb.Size = new Size(200, 30);
            foreach (string file in files)
            {
                cb.Items.Add(Path.GetFileName(file));
            }
            this.Controls.Add(cb);
        }

        private void button3_Click(object sender, EventArgs e)
        {
           // string sourceDir = @"D:/Images";
           // string backupDir = @"D:/Images/VIP";
           // MergeFile(sourceDir);
           // DirectoryInfo dirInfo = new DirectoryInfo(backupDir);

           // DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
           // dirSecurity.AddAccessRule(new FileSystemAccessRule("Gerges Bernaba",
           //     FileSystemRights.Read | FileSystemRights.Write, AccessControlType.Allow));
           // dirInfo.SetAccessControl(dirSecurity);

           //// CombineMultipleFilesIntoSingleFile(sourceDir, "*.png", backupDir);
           // string[] picList = Directory.GetFiles(sourceDir, "*.png");

           // foreach (string f in picList)
           // {
           //     // Remove path from the file name.
           //     string fName = f.Substring(sourceDir.Length + 1);

           //     // Use the Path.Combine method to safely append the file name to the path.
           //     // Will overwrite if the destination file already exists.
           //     File.Copy(Path.Combine(sourceDir, fName), Path.Combine(backupDir, fName), true);
           // }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
           
        }

        //public bool MergeFile(string inputfoldername1)
        //{
        //    bool Output = false;
        //    try
        //    {
        //        string[] tmpfiles = Directory.GetFiles(inputfoldername1, "*.png");
        //        FileStream outPutFile = null;
        //        string PrevFileName = "";
        //        foreach (string tempFile in tmpfiles)
        //        {
        //            string fileName = Path.GetFileNameWithoutExtension(tempFile);
        //          //  string baseFileName = fileName.Substring(0, fileName.IndexOf(Convert.ToChar(".")));
        //            string extension = Path.GetExtension(fileName);
        //            if (!PrevFileName.Equals(fileName))
        //            {
        //                if (outPutFile != null)
        //                {
        //                    outPutFile.Flush();
        //                    outPutFile.Close();
        //                }
        //                outPutFile = new FileStream(SaveFileFolder + "\\" + fileName + extension, FileMode.OpenOrCreate, FileAccess.Write);
        //            }
        //            int bytesRead = 0;
        //            byte[] buffer = new byte[1024];
        //            FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read);
        //            while ((bytesRead = inputTempFile.Read(buffer, 0, 1024)) > 0)
        //                outPutFile.Write(buffer, 0, bytesRead);
        //            inputTempFile.Close();
        //            File.Delete(tempFile);
        //            PrevFileName = fileName;
        //        }
        //        outPutFile.Close();
        //        label5.Text = "Files have been merged and saved at location C:\\";
        //    }
        //    catch
        //    {
        //    }
        //    return Output;

        //}

        //private static void CombineMultipleFilesIntoSingleFile(string inputDirectoryPath, string inputFileNamePattern, string outputFilePath)
        //{

        //    string[] inputFilePaths = Directory.GetFiles(inputDirectoryPath, inputFileNamePattern);
        //    Console.WriteLine("Number of files: {0}.", inputFilePaths.Length);

        //    //DirectoryInfo dirInfo = new DirectoryInfo(outputFilePath);

        //    //DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
        //    //dirSecurity.AddAccessRule(new FileSystemAccessRule("Gerges Bernaba",
        //    //    FileSystemRights.Read | FileSystemRights.Write, AccessControlType.Allow));
        //    //dirInfo.SetAccessControl(dirSecurity);
        //    using (var outputStream = File.Create(outputFilePath))
        //    {
        //        foreach (var inputFilePath in inputFilePaths)
        //        {
        //            using (var inputStream = File.OpenRead(inputFilePath))
        //            {
        //                // Buffer size can be passed as the second argument.
        //                inputStream.CopyTo(outputStream);
        //            }
        //            Console.WriteLine("The file {0} has been processed.", inputFilePath);
        //        }
        //    }
        //}
    }
}

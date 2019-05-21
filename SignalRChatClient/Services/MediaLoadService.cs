using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PLL.Services
{
    public class MediaLoadService
    {
        #region Private Fields

        private const string _defaultBlobFilesLocation = @"Download\";
        private const string _defaultChunkLocation = @"Chunks\";
        private const int _sizeOfChunk = 50000000;

        private readonly CloudBlobClient blobClient;
        private readonly CloudBlobContainer blobContainer;
        private readonly CloudStorageAccount storageAccount;

        #endregion Private Fields

        #region Public Constructors

        public MediaLoadService(string connectionString, string containerName)
        {
            if (CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                blobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = blobClient.GetContainerReference(containerName);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<string> DownloadFileAsync(string mediaName, IEnumerable<string> blobFileNames, string roomUniqName)
        {
            var result = string.Empty;
            string directoryName = _defaultBlobFilesLocation + mediaName + @"\";

            foreach (var blobFileName in blobFileNames)
            {

                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobFileName);

                if (await blockBlob.ExistsAsync())
                {
                    Directory.CreateDirectory(directoryName);

                    MemoryStream memStream = new MemoryStream();

                    await blockBlob.DownloadToStreamAsync(memStream);

                    using (FileStream fileStream = new FileStream(directoryName + blobFileName.Replace(roomUniqName + " - ", string.Empty), FileMode.OpenOrCreate))
                    {
                        memStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    result = directoryName;
                }

            }

            return result;
        }

        public bool MergeFile(string folderName, string playlistFolderPath, string baseFileName)
        {
            bool Output = false;

            try
            {
                string[] tmpfiles = Directory.GetFiles(folderName, "*.tmp");

                FileStream outPutFile = new FileStream(playlistFolderPath + "\\" + baseFileName + baseFileName.Split('.').Last(), FileMode.OpenOrCreate, FileAccess.Write);
                string PrevFileName = "";

                foreach (string tempFile in tmpfiles)
                {
                    int bytesRead = 0;
                    byte[] buffer = new byte[1024];
                    FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read);

                    while ((bytesRead = inputTempFile.Read(buffer, 0, 1024)) > 0)
                        outPutFile.Write(buffer, 0, bytesRead);

                    inputTempFile.Close();
                    System.IO.File.Delete(tempFile);
                    PrevFileName = baseFileName;
                }

                outPutFile.Close();
                Output = true;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }

            return Output;
        }

        public IEnumerable<string> UploadFile(string fullFilePath, string roomUniqName)
        {
            var result = new List<string>();
            string folderName = SplitFile(fullFilePath);
            if (!string.IsNullOrEmpty(folderName))
            {
                foreach (var tempFilePath in Directory.GetFiles(_defaultChunkLocation + folderName, "*.tmp"))
                {
                    var stream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Read);
                    var blobFileName = roomUniqName + " - " + tempFilePath.Split('\\').Last();

                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobFileName);
                    blob.UploadFromStream(stream);
                    result.Add(blobFileName);
                }
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private string SplitFile(string SourceFile)
        {
            var result = string.Empty;
            try
            {
                FileStream fileStream = new FileStream(SourceFile, FileMode.Open, FileAccess.Read);
                string baseFileName = Path.GetFileNameWithoutExtension(SourceFile);

                for (int i = 0; i <= fileStream.Length / _sizeOfChunk; i++)
                {
                    string Extension = Path.GetExtension(SourceFile);

                    string splitFilesDirectory = _defaultChunkLocation + baseFileName;
                    Directory.CreateDirectory(splitFilesDirectory);

                    FileStream outputFile = new FileStream(splitFilesDirectory + "\\" + baseFileName + "." +
                        i.ToString().PadLeft(5, Convert.ToChar("0")) + Extension + ".tmp", FileMode.Create, FileAccess.Write);

                    int bytesRead = 0;
                    byte[] buffer = new byte[_sizeOfChunk];

                    if ((bytesRead = fileStream.Read(buffer, 0, _sizeOfChunk)) > 0)
                    {
                        outputFile.Write(buffer, 0, bytesRead);

                        string packet = baseFileName + "." + i.ToString().PadLeft(3, Convert.ToChar("0")) + Extension.ToString();
                    }

                    outputFile.Close();
                }
                fileStream.Close();
                result = baseFileName;
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }

            return result;
        }

        #endregion Private Methods
    }
}
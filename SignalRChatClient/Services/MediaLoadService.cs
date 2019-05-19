using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PLL.Services
{
    public class MediaLoadService
    {
        #region Private Fields

        private const string _defaultBlobFilesLocation = @"Media\Download\";
        private const string _defaultChunkLocation = @"Media\Chunks\";
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

        public async Task<bool> DownloadFileAsync(string mediaName, string blobFileName, string playlistFolderPath, string roomUniqName)
        {
            var result = true;
            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobFileName);

            if (await blockBlob.ExistsAsync())
            {
                string directoryName = _defaultBlobFilesLocation + mediaName + @"\";
                Directory.CreateDirectory(directoryName);

                MemoryStream memStream = new MemoryStream();

                await blockBlob.DownloadToStreamAsync(memStream);

                using (FileStream fileStream = new FileStream(directoryName + blobFileName.Replace(roomUniqName + " - ", string.Empty), FileMode.OpenOrCreate))
                {
                    memStream.CopyTo(fileStream);
                    fileStream.Flush();
                }

                MergeFile(directoryName, playlistFolderPath);

                result = true;
            }

            return result;
        }

        public bool MergeFile(string folderName, string playlistFolderPath)
        {
            bool Output = false;

            try
            {
                string[] tmpfiles = Directory.GetFiles(folderName, "*.tmp");

                FileStream outPutFile = null;
                string PrevFileName = "";

                foreach (string tempFile in tmpfiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tempFile);
                    string baseFileName = fileName.Substring(0, fileName.IndexOf(Convert.ToChar(".")));
                    string extension = Path.GetExtension(fileName);

                    if (!PrevFileName.Equals(baseFileName))
                    {
                        if (outPutFile != null)
                        {
                            outPutFile.Flush();
                            outPutFile.Close();
                        }
                        outPutFile = new FileStream(playlistFolderPath + "\\" + baseFileName + extension, FileMode.OpenOrCreate, FileAccess.Write);
                    }

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

        public async Task<bool> UploadFileAsync(string fullFilePath, string roomUniqName)
        {
            var result = false;
            string folderName = SplitFile(fullFilePath);

            if (!string.IsNullOrEmpty(folderName))
            {
                foreach (var tempFilePath in Directory.GetFiles(_defaultChunkLocation + folderName, "*.tmp"))
                {
                    var stream = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Read);
                    var blobFileName = roomUniqName + " - " + tempFilePath.Split('\\').Last();

                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobFileName);
                    await blob.UploadFromStreamAsync(stream);
                }
                result = true;
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
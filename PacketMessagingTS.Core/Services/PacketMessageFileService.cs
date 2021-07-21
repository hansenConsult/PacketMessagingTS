using System;
using System.IO;
using System.Xml.Serialization;

using PacketMessagingTS.Core.Contracts.Services;

namespace PacketMessagingTS.Core.Services
{
    class PacketMessageFileService : IFileService
    {


        public PacketMessage Read<PacketMessage>(string folderPath, string fileName)
        {
            //StreamReader reader;
            PacketMessage packetMessage;
            try
            {
                using (var stream = new FileStream(folderPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        var serializer = new XmlSerializer(typeof(PacketMessage));
                        packetMessage = (PacketMessage)serializer.Deserialize(reader);
                    }
                }
                return packetMessage;
            }
            catch (Exception e)
            {
                throw;
                //_logHelper.Log(LogLevel.Error, $"Failed to open {file?.Path}, {e}");
            }
            //return default;
        }

        public void Save<T>(string folderPath, string fileName, T content)
        {
            string filePath = Path.Combine(folderPath, fileName);
            try
            {
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    using (TextWriter writer = new StreamWriter(stream))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(PacketMessage));
                        serializer.Serialize(writer, this);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
                //_logHelper.Log(LogLevel.Error, $"Failed to save {filePath}, {e.Message}");
            }

            // Now replace tab characters with escaped tab character
            string fileBuffer = "";
            bool tabCharacterFound = false;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open)))
                {
                    // Read the stream to a string, and write the string to the console.
                    fileBuffer = sr.ReadToEnd();
                    int index = fileBuffer.IndexOf('\t');
                    if (index < 0)
                        return;

                    tabCharacterFound = true;
                    fileBuffer = fileBuffer.Replace("\t", "&#x9;");
                }
            }
            catch (Exception e)
            {
                throw;
                //_logHelper.Log(LogLevel.Error, $"Failed to read {filePath} for substituting tab characters, {e.Message}");
            }

            if (tabCharacterFound)
            {
                try
                {
                    // Write xml file back with escaped tab characters
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        using (StreamWriter outputFile = new StreamWriter(stream))
                        {
                            outputFile.Write(fileBuffer);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw;
                    //_logHelper.Log(LogLevel.Error, $"Failed to write {filePath} with escaped tab characters {e.Message}");
                }
            }
        }



        public void Delete(string folderPath, string fileName)
        {
            if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
            {
                File.Delete(Path.Combine(folderPath, fileName));
            }
        }

    }
}

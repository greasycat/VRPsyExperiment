using System;
using System.Collections.Generic;
using System.IO;
using Experiment;

namespace IO
{
    public enum CsvFileMode
    {
        CsvRead,
        CsvWrite,
        CsvCreate,
        CsvNothing,
    }
    
    public class Csv: IDisposable
    {
        public string FileName {private set; get; }
        public int SkipRows { private set; get; }
        public bool CreateIfNonExist { private set; get; }

        private readonly CsvFileMode fileModeState;
        private FileStream fileStream;
        private StreamReader streamReader;
        private StreamWriter streamWriter;
        
        
        public Csv(string filename, CsvFileMode fileModeState = CsvFileMode.CsvNothing, int skipRows = 0, bool createIfNonExist = false)
        {
            FileName = filename;
            this.fileModeState = fileModeState;
            this.SkipRows = skipRows;
            this.CreateIfNonExist = createIfNonExist;
            OpenFile();
        }

        private void OpenFile()
        {
            switch (fileModeState)
            {
                case CsvFileMode.CsvRead:
                    fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    streamReader = new StreamReader(fileStream);
                    for (var i = 0; i!= SkipRows && streamReader.ReadLine() != null; ++i) {}
                    break;
                case CsvFileMode.CsvWrite:
                    fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Write, FileShare.Write);
                    streamWriter = new StreamWriter(fileStream);
                    break;
                case CsvFileMode.CsvCreate:
                    fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                    streamWriter = new StreamWriter(fileStream);
                    break;
                case CsvFileMode.CsvNothing:
                    break;
                default:
                    throw new CsvIOException("Incorrect IO state");
            }
            
        }

        public string ReadRow()
        {
            if (fileModeState != CsvFileMode.CsvRead)
                throw new CsvIOException("Wrong file IO state");

            if (streamReader == null)
                throw new CsvIOException("StreamReader is null");
            
            return streamReader.ReadLine();
        }

        public void WriteRow(string row)
        {
            if (fileModeState != CsvFileMode.CsvWrite|| fileModeState!= CsvFileMode.CsvCreate)
                throw new CsvIOException("Wrong file IO state");

            if (streamWriter == null)
                throw new CsvIOException("StreamWriter is null");
            
            streamWriter.WriteLine(row);
        }

        public IEnumerable<TrialInfo> ReadTrialInfo<T>() where T:TrialInfo
        {
            if (fileModeState != CsvFileMode.CsvRead)
                throw new CsvIOException("Wrong file IO state");

            if (streamReader == null)
                throw new CsvIOException("StreamReader is null");
            
            string line;
            while ((line = streamReader.ReadLine()) != null)
                yield return TrialInfoFactory.FromCsvRow<T>(line);
        }

        public void Dispose()
        {
            fileStream?.Dispose();
            streamReader?.Dispose();
            streamWriter?.Dispose();
        }

    }

    public class CsvIOException : Exception
    {
        public CsvIOException(string Msg): base(Msg) {}
    }
}
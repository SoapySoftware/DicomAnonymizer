using FellowOakDicom.Imaging.Render;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging;
using FellowOakDicom;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DicomAnon2
{
    public partial class Form1 : Form
    {
        private string _strFolder;
        private List<string> _lFiles;
        private BackgroundWorker backgroundWorkerAnon;
        private BackgroundWorker backgroundWorkerGetData;
        private BackgroundWorker backgroundWorkerMakePNGs;

        public Form1()
        {
            InitializeComponent();

            _strFolder = null;
            _lFiles = new List<string>();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\DicomAnon"))
            {
                //if it does exist, retrieve the stored values  
                if (key != null)
                {
                    string dir = key.GetValue("LastDirectory").ToString();
                    if (Directory.Exists(dir))
                    {
                        folderBrowserDialog1.SelectedPath = dir;
                    }
                }
            }

            backgroundWorkerAnon = new BackgroundWorker();
            backgroundWorkerAnon.WorkerReportsProgress = true;
            backgroundWorkerAnon.WorkerSupportsCancellation = true;

            backgroundWorkerGetData = new BackgroundWorker();
            backgroundWorkerGetData.WorkerReportsProgress = true;
            backgroundWorkerGetData.WorkerSupportsCancellation = true;

            backgroundWorkerMakePNGs = new BackgroundWorker();
            backgroundWorkerMakePNGs.WorkerReportsProgress = true;
            backgroundWorkerMakePNGs.WorkerSupportsCancellation = true;

            new DicomSetupBuilder()
  .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>())
  .SkipValidation()
  .Build();

            InitializeBackgroundWorkers();
        }

        private void InitializeBackgroundWorkers()
        {
            backgroundWorkerAnon.DoWork += new DoWorkEventHandler(backgroundWorkerAnon_DoWork);
            backgroundWorkerAnon.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorkerAnon.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);

            backgroundWorkerGetData.DoWork += new DoWorkEventHandler(backgroundWorkerGetData_DoWork);
            backgroundWorkerGetData.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorkerGetData.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);

            backgroundWorkerMakePNGs.DoWork += new DoWorkEventHandler(backgroundWorkerMakePNGs_DoWork);
            backgroundWorkerMakePNGs.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorkerMakePNGs.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);

        }

        private void buttonAnon_Click(object sender, EventArgs e)
        {
            if (buttonAnon.Enabled == false)
            {
                return;
            }

            buttonAnon.Enabled = false;
            buttonGetData.Enabled = false;
            buttonMakePNGs.Enabled = false;

            richTextBoxOut.Clear();
            richTextBoxOut.AppendText("Beginning anonymisation.");
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            int nTop, nRight, nBottom, nLeft;
            if (!int.TryParse(textCropTop.Text, out nTop))
            {
                nTop = 0;
            }
            if (!int.TryParse(textCropRight.Text, out nRight))
            {
                nRight = 0;
            }
            if (!int.TryParse(textCropBottom.Text, out nBottom))
            {
                nBottom = 0;
            }
            if (!int.TryParse(textCropLeft.Text, out nLeft))
            {
                nLeft = 0;
            }

            nTop = Math.Max(0, nTop);
            nRight = Math.Max(0, nRight);
            nBottom = Math.Max(0, nBottom);
            nLeft = Math.Max(0, nLeft);

            Tuple<int, int, int, int> lCrop = new Tuple<int, int, int, int>(nTop, nRight, nBottom, nLeft);

            if (Directory.Exists(_strFolder + "\\Anonymised"))
            {
                richTextBoxOut.AppendText("\nCleaning up existing subfolder 'Anonymised'.");
                DirectoryInfo di = new DirectoryInfo(_strFolder + "\\Anonymised");
                di.Delete(true);

            }

            Directory.CreateDirectory(_strFolder + "\\Anonymised");
            richTextBoxOut.AppendText("\nSubfolder 'Anonymised' successfully created.");

            Directory.CreateDirectory(_strFolder + "\\Anonymised\\TIF");
            Directory.CreateDirectory(_strFolder + "\\Anonymised\\PNG");
            Directory.CreateDirectory(_strFolder + "\\Anonymised\\DICOM");

            Tuple<string, Tuple<int, int, int, int>, List<string>, bool> param = new Tuple<string, Tuple<int, int, int, int>, List<string>, bool>(_strFolder, lCrop, _lFiles, chkKeepFilenames.Checked);

            // Start the asynchronous operation.
            backgroundWorkerAnon.RunWorkerAsync(param);
        }

        private void buttonGetData_Click(object sender, EventArgs e)
        {
            if (buttonGetData.Enabled == false)
            {
                return;
            }

            buttonAnon.Enabled = false;
            buttonGetData.Enabled = false;
            buttonMakePNGs.Enabled = false;

            richTextBoxOut.Clear();
            richTextBoxOut.AppendText("Getting DICOM data.");
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            if (!Directory.Exists(_strFolder + "\\Database"))
            {
                Directory.CreateDirectory(_strFolder + "\\Database");
            }


            richTextBoxOut.AppendText("\nSubfolder 'Database' successfully created.");

            Tuple<string, List<string>> param = new Tuple<string, List<string>>(_strFolder, _lFiles);

            // Start the asynchronous operation.
            backgroundWorkerGetData.RunWorkerAsync(param);

        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _strFolder = folderBrowserDialog1.SelectedPath;

                //accessing the CurrentUser root element  
                //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\DicomAnon"))
                {
                    key.SetValue("LastDirectory", _strFolder);
                }

                GetFileDetails();
            }
        }

        private void checkBoxIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
        {
            if (_strFolder != null)
            {
                GetFileDetails();
            }
        }

        private void GetFileDetails()
        {
            SearchOption opt;

            labelWarnings.Text = "Warnings:";

            if (checkBoxIncludeSubfolders.Checked)
            {
                opt = SearchOption.AllDirectories;
            }
            else
            {
                opt = SearchOption.TopDirectoryOnly;
            }

            string[] files = new string[] { };

            try
            {
                this.Cursor = Cursors.WaitCursor;
                files = Directory.GetFiles(_strFolder, "*.dcm", opt);

            }
            catch (Exception ex)
            {
                labelWarnings.Text += "\n* " + ex.Message;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }


            _lFiles.Clear();

            int nAnon = 0;
            int nRecoded = 0;

            foreach (string f in files)
            {
                string dir = Path.GetDirectoryName(f);
                if (Path.GetDirectoryName(f).StartsWith(_strFolder + "\\Anonymised"))
                {
                    nAnon++;
                }
                else if (Path.GetDirectoryName(f).StartsWith(_strFolder + "\\Recoded"))
                {
                    nRecoded++;
                }
                else
                {
                    _lFiles.Add(f);
                }
            }

            labelFolder.Text = "Folder selected: " + _strFolder;
            labelImages.Text = "Images (*.dcm) found: " + (_lFiles.Count == 0 ? "None" : _lFiles.Count.ToString());

            if (Directory.Exists(_strFolder + "\\Anonymised"))
            {
                labelWarnings.Text += "\n* Anonymised subfolder will be destroyed if you anon";

                if (nAnon > 0)
                {
                    labelWarnings.Text += "\n* Anonymised subfolder contains " + nAnon + " images.";
                }
            }

            if (Directory.Exists(_strFolder + "\\Recoded"))
            {
                labelWarnings.Text += "\n* Recoded subfolder will be destroyed if you recode";

                if (nAnon > 0)
                {
                    labelWarnings.Text += "\n* Recoded subfolder contains " + nRecoded + " images.";
                }
            }

            if (_lFiles.Count == 0)
            {
                labelWarnings.Text += "\n* No DICOM (*.dcm) files were found.";
                buttonAnon.Enabled = false;
                buttonGetData.Enabled = false;
                buttonMakePNGs.Enabled = false;
            }
            else
            {
                buttonAnon.Enabled = true;
                buttonGetData.Enabled = true;
                buttonMakePNGs.Enabled = true;
            }
        }

        private void backgroundWorkerAnon_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = AnonymizeDCMs((Tuple<string, Tuple<int, int, int, int>, List<string>, bool>)e.Argument, worker, e);
        }

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                richTextBoxOut.AppendText("\n" + e.Error.Message);
            }
            else if (e.Cancelled)
            {
                richTextBoxOut.AppendText("\nCanceled");
            }
            else
            {
                // Finally, handle the case where the operation 
                // succeeded.
                richTextBoxOut.AppendText("\n" + e.Result.ToString());
            }

            buttonAnon.Enabled = true;
            buttonGetData.Enabled = true;
            buttonMakePNGs.Enabled = true;
            progressBar.Value = 0;
        }

        // This event handler updates the progress bar.
        private void backgroundWorker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        string AnonymizeDCMs(Tuple<string, Tuple<int, int, int, int>, List<string>, bool> param, BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<Tuple<string, string, string>> lFailed = new List<Tuple<string, string, string>>();
            string strPath = param.Item1;
            Tuple<int, int, int, int> lCrop = param.Item2;
            List<string> files = param.Item3;
            bool bKeepFilenames = param.Item4;

            bool bCrop = lCrop.Item1 + lCrop.Item2 + lCrop.Item3 + lCrop.Item4 > 0 ? true : false;

            DicomUIDGenerator uidGen = new DicomUIDGenerator();
            List<Tuple<string, string, string, string, string>> listDCM = new List<Tuple<string, string, string, string, string>>();

            int i = 0, k = 0;

            // Randomize input list
            Random rand = new Random();
            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (k = 0; k < files.Count - 1; k++)
            {
                int j = rand.Next(k, files.Count);
                string temp = files[k];
                files[k] = files[j];
                files[j] = temp;
            }

            DateTime dt = DateTime.Now;


            int nSuccess = 0;
            foreach (string strFile in files)
            {

                i++;
                DicomFile file;
                try
                {
                    file = DicomFile.Open(strFile);
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "Does not exist", "\"" + ex.Message.Replace("\"", "\"\"") + "\""));
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    continue;
                }

                if (file == null)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "Does exists but is empty", "File is null"));
                    System.Diagnostics.Debug.WriteLine("File is null");
                    continue;
                }

                string strOriginalPatientID = "";

                try
                {
                    GetDicomTagString(ref file, DicomTag.PatientID, out strOriginalPatientID);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

                // A batch of already semi-anonymized files were sent that were missing critical DICOM fields.
                string strSOPInstanceUID;
                GetDicomTagString(ref file, DicomTag.SOPInstanceUID, out strSOPInstanceUID);
                if (strSOPInstanceUID.Length == 0)
                {
                    file.Dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
                }

                DicomAnonymizer anon = new DicomAnonymizer();
                DicomFile fileAnon;
                try
                {
                    fileAnon = anon.Anonymize(file);
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "Anonymizing", "\"" + ex.Message.Replace("\"", "\"\"") + "\""));
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    //continue;
                    fileAnon = file;
                }


                DicomTag[] tagsToRemove = { DicomTag.StudyDate, DicomTag.StudyTime, DicomTag.PatientID, DicomTag.StudyID, DicomTag.StudyInstanceUID, DicomTag.PlateID };

                foreach (DicomTag d in tagsToRemove)
                {
                    try
                    {
                        fileAnon.Dataset.Remove(d);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error removing element: " + ex.ToString());
                    }
                }


                fileAnon.Dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
                fileAnon.Dataset.Add(DicomTag.StudyDate, dt.Year.ToString("0000") + dt.Month.ToString("00") + dt.Day.ToString("00"));
                fileAnon.Dataset.Add(DicomTag.StudyTime, dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00"));
                fileAnon.Dataset.Add(DicomTag.PatientID, i.ToString());
                fileAnon.Dataset.Add(DicomTag.StudyID, i.ToString());

                string strStudyID = "";

                if (bKeepFilenames)
                {
                    strStudyID = Path.GetFileNameWithoutExtension(strFile);
                }
                else
                {
                    GetDicomTagString(ref fileAnon, DicomTag.StudyInstanceUID, out strStudyID);
                }

                DicomPixelData header = null;
                IPixelData pixelData = null;
                bool bRecode = false;
                bool bHasPixels = false;

                try
                {
                    header = DicomPixelData.Create(fileAnon.Dataset);
                    pixelData = PixelDataFactory.Create(header, header.NumberOfFrames - 1);
                    bHasPixels = true;
                }
                catch
                {
                    bRecode = true;
                }

                if(bRecode)
                {
                    try
                    {
                        // Recode DCM
                        var transcoder = new DicomTranscoder(fileAnon.Dataset.InternalTransferSyntax, DicomTransferSyntax.ImplicitVRLittleEndian);
                        var newFile = transcoder.Transcode(fileAnon);
                        fileAnon = newFile;

                        header = DicomPixelData.Create(fileAnon.Dataset);
                        pixelData = PixelDataFactory.Create(header, header.NumberOfFrames - 1);
                        bHasPixels = true;
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to recode bad image");
                        System.Diagnostics.Debug.WriteLine(fileAnon.Dataset.InternalTransferSyntax.ToString());
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }


                if(bHasPixels)
                {
                    try
                    {

                        int rows = header.Height;
                        int columns = header.Width;

                        Array a;
                        byte[] result;

                        bool b16bit = false;

                        if (pixelData is GrayscalePixelDataU16)
                        {
                            ushort[] pixels = ((GrayscalePixelDataU16)pixelData).Data;
                            a = pixels;
                            b16bit = true;
                        }
                        else if (pixelData is GrayscalePixelDataS16)
                        {
                            short[] pixels = ((GrayscalePixelDataS16)pixelData).Data;
                            a = pixels;
                            b16bit = true;
                        }
                        else if (pixelData is GrayscalePixelDataU32)
                        {
                            uint[] pixels = ((GrayscalePixelDataU32)pixelData).Data;
                            a = pixels;
                        }
                        else if (pixelData is GrayscalePixelDataS32)
                        {
                            int[] pixels = ((GrayscalePixelDataS32)pixelData).Data;
                            a = pixels;
                        }
                        else if (pixelData is GrayscalePixelDataU8)
                        {
                            byte[] pixels = ((GrayscalePixelDataU8)pixelData).Data;
                            a = pixels;
                        }
                        else
                        {
                            throw new Exception("DICOM image format not supported (this program only supports greyscale).");
                        }

                        // Can't seem to figure out the byte formatting between 16-bit greyscale DCM versus C#'s 16-bit greyscale.
                        //b16bit = false;

                        if (bCrop)
                        {

                            // Top
                            if (lCrop.Item1 > 0)
                            {
                                Array cropped = Array.CreateInstance(a.GetValue(0).GetType(), a.Length - (columns * lCrop.Item1));
                                Array.Copy(a, columns * lCrop.Item1, cropped, 0, cropped.Length);
                                a = cropped;
                                rows -= lCrop.Item1;
                            }

                            // Right
                            if (lCrop.Item2 > 0)
                            {
                                Array cropped = Array.CreateInstance(a.GetValue(0).GetType(), a.Length - (rows * lCrop.Item2));

                                for (k = 0; k < rows; k++)
                                {
                                    Array.Copy(a, k * columns, cropped, k * (columns - lCrop.Item2), columns - lCrop.Item2);
                                }

                                a = cropped;
                                columns -= lCrop.Item2;
                            }

                            // Bottom
                            if (lCrop.Item3 > 0)
                            {
                                Array cropped = Array.CreateInstance(a.GetValue(0).GetType(), a.Length - (columns * lCrop.Item3));
                                Array.Copy(a, 0, cropped, 0, cropped.Length);
                                a = cropped;
                                rows -= lCrop.Item3;
                            }

                            // Left
                            if (lCrop.Item4 > 0)
                            {
                                Array cropped = Array.CreateInstance(a.GetValue(0).GetType(), a.Length - (rows * lCrop.Item4));

                                for (k = 0; k < rows; k++)
                                {
                                    Array.Copy(a, k * columns + lCrop.Item4, cropped, k * (columns - lCrop.Item4), columns - lCrop.Item4);
                                }

                                a = cropped;
                                columns -= lCrop.Item4;
                            }

                            // Now we need to copy the Array "a" into a byte array.  
                            // But first!  Should we make sure that it's actually a 16-bit array?
                            int nBytes = a.Length * System.Runtime.InteropServices.Marshal.SizeOf(a.GetValue(0));
                            result = new byte[nBytes];
                            Buffer.BlockCopy(a, 0, result, 0, nBytes);

                            FellowOakDicom.IO.Buffer.MemoryByteBuffer buffer = new FellowOakDicom.IO.Buffer.MemoryByteBuffer(result);
                            DicomDataset dataset = new DicomDataset();

                            dataset = fileAnon.Dataset.Clone();

                            dataset.AddOrUpdate(DicomTag.Rows, (ushort)rows);
                            dataset.AddOrUpdate(DicomTag.Columns, (ushort)columns);

                            DicomPixelData newPixelData = DicomPixelData.Create(dataset, true);
                            newPixelData.BitsStored = header.BitsStored;
                            newPixelData.SamplesPerPixel = header.SamplesPerPixel;
                            newPixelData.HighBit = header.HighBit;
                            newPixelData.PhotometricInterpretation = header.PhotometricInterpretation;
                            newPixelData.PixelRepresentation = header.PixelRepresentation;
                            newPixelData.PlanarConfiguration = header.PlanarConfiguration;
                            newPixelData.Height = (ushort)rows;
                            newPixelData.Width = (ushort)columns;
                            newPixelData.AddFrame(buffer);

                            fileAnon = new DicomFile(dataset);

                        }

                        // Only do this if it's a 16bit file that we want a 16bit png for
                        /*if (b16bit)
                        {
                            int nBytes = a.Length * System.Runtime.InteropServices.Marshal.SizeOf(a.GetValue(0));
                            result = new byte[nBytes];

                            // If we're using a format that's "16bit" but actually less, scale the values?
                            if (header.BitsStored < header.BitsAllocated)
                            {
                                int nShift = header.BitsAllocated - header.BitsStored;
                                int nFlag = (0x1 << header.BitsStored) - 1;
                                for (k = 0; k < a.Length; k++)
                                {
                                    a.SetValue((ushort)(((nFlag - ((ushort)a.GetValue(k) & nFlag)) << nShift) & 0xFFFF), k);
                                }
                            }

                            Buffer.BlockCopy(a, 0, result, 0, nBytes);

                            unsafe
                            {
                                fixed (byte* ptr = result)
                                {
                                    using (Bitmap img16 = new Bitmap(columns, rows, 4 * ((2 * columns + 3) / 4), System.Drawing.Imaging.PixelFormat.Format16bppGrayScale, new IntPtr(ptr)))
                                    {
                                        SaveBmp(img16, strPath + "/Anonymised/TIF/" + i + ".tif");
                                        //img16.Save(strPath + "/Anonymised/" + strStudyID + "-16bitGreyscale.png");
                                    }
                                }
                            }
                        }*/

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to crop image");
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }               

                string sf = strPath + "/Anonymised/DICOM/" + strStudyID + ".dcm";
                
                try
                {

                    fileAnon.Save(sf);

                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "Header non-standard/corrupt", CSVEscape(ex.Message)));
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    continue;
                }





                DicomImage img;

                try
                {
                    //var img = new DicomImage(strPath + "/Anonymised/" + strStudyID + ".dcm");
                    img = new DicomImage(sf);

                    listDCM.Add(new Tuple<string, string, string, string, string>(i.ToString(), strStudyID, strFile, strOriginalPatientID, "Success"));
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "No image data in the DICOM file", CSVEscape(ex.Message)));
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    continue;
                }

                // Convert DCM to a 32-bit per pixel (8-bit per each color RGB + 8-bit unused alpha) PNG file
                try
                {
                    FellowOakDicom.IO.PinnedIntArray px = img.RenderImage().Pixels;

                    //Bitmap bbb = img.RenderImage().AsClonedBitmap();

                    int[] pxi = px.Data;

                    byte[] result = new byte[px.ByteSize];
                    Buffer.BlockCopy(pxi, 0, result, 0, result.Length);

                    unsafe
                    {
                        fixed (byte* ptr = result)
                        {
                            using (Bitmap image = new Bitmap(img.Width, img.Height, img.Width * 4,
                                System.Drawing.Imaging.PixelFormat.Format32bppRgb, new IntPtr(ptr)))
                            {
                                //image.Save(strPath + "/Anonymised/PNG/" + i + ".png");
                                image.Save(strPath + "/Anonymised/PNG/" + strStudyID + ".png");

                                //image.Save(strPath + "/Anonymised/PNG/" + strOriginalPatientID + "-" + n.ToString("00") + Path.GetFileName(strFile).Substring(0, 2) + ".png");
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }


                worker.ReportProgress(i * 100 / files.Count);
                nSuccess++;

                //Console.WriteLine("Anonymized image " + i + " (of " + nFrames + " frame" + (nFrames == 1 ? "" : "s") + "): " + strFile);
            }

            // Create a map file

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(new FileStream(strPath + "/Anonymised/Map.csv", FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {
                file.WriteLine("NewPatientID,NewStudyInstanceUID,OriginalFile,OriginalPatientID,Success");
                foreach (Tuple<string, string, string, string, string> line in listDCM)
                {
                    file.WriteLine(line.Item1 + "," + line.Item2 + "," + line.Item3 + "," + line.Item4 + "," + line.Item5);
                }
            }

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(new FileStream(strPath + "/Anonymised/Failed.csv", FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {
                file.WriteLine("File,ErrorType,Exception");
                foreach (Tuple<string, string, string> line in lFailed)
                {
                    file.WriteLine(line.Item1 + "," + line.Item2 + "," + line.Item3);
                }
            }

            string strRet = nSuccess.ToString() + " images successfully anonymised, Map.csv created.\nOutput at:\n" + strPath + "\\Anonymised";
            if (lFailed.Count > 0)
            {
                strRet += "\nTSome files failed to anonymise.  See Failed.csv.";
            }
            return strRet;

        }


        private static void SaveBmp(Bitmap bmp, string path)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            var pixelFormats = ConvertBmpPixelFormat(bmp.PixelFormat);

            BitmapSource source = BitmapSource.Create(bmp.Width,
                                                      bmp.Height,
                                                      bmp.HorizontalResolution,
                                                      bmp.VerticalResolution,
                                                      pixelFormats,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * bmp.Height,
                                                      bitmapData.Stride);

            bmp.UnlockBits(bitmapData);


            FileStream stream = new FileStream(path, FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);

            stream.Close();
        }

        private static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgr32;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }

            return pixelFormats;
        }

        static bool GetDicomTagString(ref DicomFile file, DicomTag tag, out string value)
        {
            value = "";

            try
            {
                if(file.Dataset.Contains(tag))
                {
                    value = file.Dataset.GetValue<string>(tag, 0);
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch
            {
                return false;
            }
        }

        private void backgroundWorkerGetData_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = GetDataFromDCM((Tuple<string, List<string>>)e.Argument, worker, e);
        }

        string GetDataFromDCM(Tuple<string, List<string>> param, BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> lFailed = new List<string>();
            string strPath = param.Item1;
            List<string> files = param.Item2;

            List<List<string>> listDCM = new List<List<string>>();

            List<Dictionary<string, string>> lDataRaw = new List<Dictionary<string, string>>();
            Dictionary<string, int> hashFieldCount = new Dictionary<string, int>();

            int i = 0;

            DateTime dt = DateTime.Now;


            int nSuccess = 0;
            foreach (string strFile in files)
            {

                i++;
                DicomFile file;
                try
                {
                    file = DicomFile.Open(strFile);
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(strFile);
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    continue;
                }

                if (file == null)
                {
                    // Transmit message back with worker?
                    lFailed.Add(strFile);
                    System.Diagnostics.Debug.WriteLine("File is null?!");
                    continue;
                }

                List<string> lRow = new List<string>();

                string strPatientName;
                string strPatientID;
                string strPatientDOB;
                string strPatientSex;
                string strPatientAge;
                string strStudyUID;
                string strStudyDate;
                string strManufacturer;
                string strManufacturerModel;
                string strAnonymizable = "Yes";
                string strNonStandardImage = "Not opened";

                GetDicomTagString(ref file, DicomTag.PatientName, out strPatientName);
                GetDicomTagString(ref file, DicomTag.PatientID, out strPatientID);
                GetDicomTagString(ref file, DicomTag.PatientBirthDate, out strPatientDOB);
                GetDicomTagString(ref file, DicomTag.PatientSex, out strPatientSex);
                GetDicomTagString(ref file, DicomTag.PatientAge, out strPatientAge);
                GetDicomTagString(ref file, DicomTag.StudyInstanceUID, out strStudyUID);
                GetDicomTagString(ref file, DicomTag.StudyDate, out strStudyDate);
                GetDicomTagString(ref file, DicomTag.Manufacturer, out strManufacturer);
                GetDicomTagString(ref file, DicomTag.ManufacturerModelName, out strManufacturerModel);


                string strSOPInstanceUID;
                GetDicomTagString(ref file, DicomTag.SOPInstanceUID, out strSOPInstanceUID);
                if (strSOPInstanceUID.Length == 0)
                {
                    file.Dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
                }


                DicomFile fileAnon = null;
                try
                {
                    DicomAnonymizer anon = new DicomAnonymizer();
                    fileAnon = anon.Anonymize(file);
                }
                catch (Exception ex)
                {
                    strAnonymizable = "No: " + ex.Message;
                }

                if (fileAnon != null)
                {
                    try
                    {
                        var header = DicomPixelData.Create(fileAnon.Dataset);
                        var pixelData = PixelDataFactory.Create(header, header.NumberOfFrames - 1);
                        strNonStandardImage = "Standard";
                    }
                    catch (Exception ex)
                    {
                        strNonStandardImage = "Non-standard: " + ex.Message;
                    }
                }



                lRow.Add(strFile);
                lRow.Add(strStudyUID);
                lRow.Add(strPatientName);
                lRow.Add(strPatientID);
                lRow.Add(strPatientDOB);
                lRow.Add(strPatientSex);
                lRow.Add(strPatientAge);
                lRow.Add(strStudyDate);
                lRow.Add(strManufacturer);
                lRow.Add(strManufacturerModel);
                lRow.Add(CSVEscape(strAnonymizable));
                lRow.Add(CSVEscape(strNonStandardImage));
                listDCM.Add(lRow);

                Dictionary<string, string> hashRow = new Dictionary<string, string>();
                string strFileSyntax = file.Dataset.InternalTransferSyntax.ToString();
                string strColumn;
                string strValue;
                hashRow["FileName"] = strFile;
                hashRow["InternalTransferSyntax"] = strFileSyntax;
                hashRow["Anonymizable"] = strAnonymizable;
                hashRow["NonStandardImage"] = strNonStandardImage;

                foreach (DicomItem di in file.Dataset)
                {
                    strColumn = di.ToString();
                    GetDicomTagString(ref file, di.Tag, out strValue);

                    hashRow[strColumn] = strValue;

                    if (hashFieldCount.ContainsKey(strColumn))
                    {
                        hashFieldCount[strColumn]++;
                    }
                    else
                    {
                        hashFieldCount[strColumn] = 1;
                    }
                }
                lDataRaw.Add(hashRow);


                worker.ReportProgress(i * 100 / files.Count);
                nSuccess++;

                //Console.WriteLine("Anonymized image " + i + " (of " + nFrames + " frame" + (nFrames == 1 ? "" : "s") + "): " + strFile);
            }

            // Create a map file

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(new FileStream(strPath + "/Database/Database.csv", FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {
                file.WriteLine("File,StudyUID,Name,WorkerID,DOB,Sex,Age,StudyDate,Manufacturer,Model,Anonymizable,NonStandardImage");
                foreach (List<string> line in listDCM)
                {
                    string strRow = "";
                    foreach (string strCol in line)
                    {
                        strRow += (strRow.Length == 0 ? "" : ",") + (strCol.Contains(",") ? "\"" + strCol.Replace("\"", "\"\"") + "\"" : strCol);
                    }
                    file.WriteLine(strRow);
                }
            }

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(new FileStream(strPath + "/Database/DatabaseRaw.csv", FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {

                string strColumns = "FileName,InternalTransferSyntax,Anonymizable,NonStandardImage";


                List<string> lFieldsSorted = (from entry in hashFieldCount orderby entry.Value descending select entry.Key).ToList();

                foreach (string strCol in lFieldsSorted)
                {
                    strColumns += "," + CSVEscape(strCol);
                }

                file.WriteLine(strColumns);
                foreach (Dictionary<string, string> dRow in lDataRaw)
                {
                    string strRow = CSVEscape(dRow["FileName"]) + "," + CSVEscape(dRow["InternalTransferSyntax"]) + "," + CSVEscape(dRow["Anonymizable"]) + "," + CSVEscape(dRow["NonStandardImage"]);

                    foreach (string strCol in lFieldsSorted)
                    {
                        if (dRow.ContainsKey(strCol))
                        {
                            strRow += "," + CSVEscape(dRow[strCol]);
                        }
                        else
                        {
                            strRow += ",";
                        }
                    }
                    file.WriteLine(strRow);
                }
            }

            string strRet = nSuccess.ToString() + " images successfully read, Database.csv created.\nOutput at:\n" + strPath + "\\Database";
            if (lFailed.Count > 0)
            {
                strRet += "\nThese files failed to read:";
                foreach (string sf in lFailed)
                {
                    strRet += "\n" + sf;
                }
            }
            return strRet;

        }


        private string CSVEscape(string str)
        {
            if(string.IsNullOrWhiteSpace(str))
            {
                return "";
            }

            return "\"" + str.Replace("\"", "\"\"") + "\"";
        }

        private void buttonMakePNGs_Click(object sender, EventArgs e)
        {
            if (buttonMakePNGs.Enabled == false)
            {
                return;
            }

            buttonAnon.Enabled = false;
            buttonGetData.Enabled = false;
            buttonMakePNGs.Enabled = false;

            richTextBoxOut.Clear();
            richTextBoxOut.AppendText("Making PNGs.");
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            if (!Directory.Exists(_strFolder + "\\PNG"))
            {
                Directory.CreateDirectory(_strFolder + "\\PNG");
            }


            richTextBoxOut.AppendText("\nSubfolder 'PNGs' successfully created.");

            Tuple<string, List<string>> param = new Tuple<string, List<string>>(_strFolder, _lFiles);

            // Start the asynchronous operation.
            backgroundWorkerMakePNGs.RunWorkerAsync(param);
        }

        private void backgroundWorkerMakePNGs_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = MakePNGs((Tuple<string, List<string>>)e.Argument, worker, e);
        }

        string MakePNGs(Tuple<string, List<string>> param, BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> lFailed = new List<string>();
            string strPath = param.Item1;
            List<string> files = param.Item2;

            int i = 0;

            DateTime dt = DateTime.Now;

            int nSuccess = 0;
            foreach (string strFile in files)
            {

                i++;
                DicomFile file;
                try
                {
                    file = DicomFile.Open(strFile);
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(strFile);
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    continue;
                }

                if (file == null)
                {
                    // Transmit message back with worker?
                    lFailed.Add(strFile);
                    System.Diagnostics.Debug.WriteLine("File is empty");
                    continue;
                }

                DicomImage img;

                try
                {
                    img = new DicomImage(strFile);
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(strFile + ": No image data in the DICOM file - " + CSVEscape(ex.Message));
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    continue;
                }

                try
                {
                    FellowOakDicom.IO.PinnedIntArray px = img.RenderImage().Pixels;

                    //Bitmap bbb = img.RenderImage().AsClonedBitmap();

                    int[] pxi = px.Data;

                    byte[] result = new byte[px.ByteSize];
                    Buffer.BlockCopy(pxi, 0, result, 0, result.Length);

                    unsafe
                    {
                        fixed (byte* ptr = result)
                        {
                            using (Bitmap image = new Bitmap(img.Width, img.Height, img.Width * 4,
                                System.Drawing.Imaging.PixelFormat.Format32bppRgb, new IntPtr(ptr)))
                            {
                                //image.Save(strPath + "/Anonymised/PNG/" + i + ".png");

                                string strSaveTo = strPath + "/PNG/" + Path.GetFileNameWithoutExtension(strFile) + ".png";
                                System.Diagnostics.Debug.WriteLine(strSaveTo);
                                image.Save(strSaveTo);

                                //image.Save(strPath + "/Anonymised/PNG/" + strOriginalPatientID + "-" + n.ToString("00") + Path.GetFileName(strFile).Substring(0, 2) + ".png");
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }


                worker.ReportProgress(i * 100 / files.Count);
                nSuccess++;

                //Console.WriteLine("Anonymized image " + i + " (of " + nFrames + " frame" + (nFrames == 1 ? "" : "s") + "): " + strFile);
            }

            string strRet = nSuccess.ToString() + " images successfully converted to PNG";
            if (lFailed.Count > 0)
            {
                strRet += "\nThese files failed to read:";
                foreach (string sf in lFailed)
                {
                    strRet += "\n" + sf;
                }
            }
            return strRet;

        }
    }
}

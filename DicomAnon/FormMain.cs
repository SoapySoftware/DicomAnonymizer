﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Primitives;

namespace DicomAnon
{
    public partial class FormMain : Form
    {
        private string _strFolder;
        private List<string> _lFiles;
        private BackgroundWorker backgroundWorker1;
        private BackgroundWorker backgroundWorker2;
        private BackgroundWorker backgroundWorker3;

        public FormMain()
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

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;

            backgroundWorker2 = new BackgroundWorker();
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            backgroundWorker3 = new BackgroundWorker();
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;

            InitializeBackgroundWorkers();
        }

        private void InitializeBackgroundWorkers()
        {
            backgroundWorker1.DoWork +=
                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);

            backgroundWorker2.DoWork +=
    new DoWorkEventHandler(backgroundWorker2_DoWork);
            backgroundWorker2.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker2.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);

            backgroundWorker3.DoWork +=
    new DoWorkEventHandler(backgroundWorker3_DoWork);
            backgroundWorker3.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker3.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
        }

        private void ButtonSelectFolder_Click(object sender, EventArgs e)
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

        private void CheckBoxIncludeSubfolders_CheckedChanged(object sender, EventArgs e)
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
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else
            {
                buttonAnon.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void ButtonAnon_Click(object sender, EventArgs e)
        {
            if (buttonAnon.Enabled == false)
            {
                return;
            }

            buttonAnon.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;

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
            backgroundWorker1.RunWorkerAsync(param);
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker1_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = WorkerFunc((Tuple<string, Tuple<int, int, int, int>, List<string>, bool>)e.Argument, worker, e);
        }

        // This event handler deals with the results of the
        // background operation.
        private void backgroundWorker1_RunWorkerCompleted(
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
            button1.Enabled = true;
            button2.Enabled = true;
            progressBar.Value = 0;
        }

        // This event handler updates the progress bar.
        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        string WorkerFunc(Tuple<string, Tuple<int, int, int, int>, List<string>, bool> param, BackgroundWorker worker, DoWorkEventArgs e)
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
                    strOriginalPatientID = file.Dataset.GetValue<string>(DicomTag.PatientID, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

                // A batch of already semi-anonymized files were sent that were missing critical DICOM fields.
                string strSOPInstanceUID;
                GetDicomTagString(ref file, DicomTag.SOPInstanceUID, out strSOPInstanceUID);
                if(strSOPInstanceUID.Length == 0)
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


                DicomTag[] tagsToRemove = { DicomTag.StudyDate, DicomTag.StudyTime, DicomTag.PatientID, DicomTag.StudyID, DicomTag.StudyInstanceUID };

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
                    strStudyID = fileAnon.Dataset.GetValue<string>(DicomTag.StudyInstanceUID, 0);
                }

                try
                {

                    var header = DicomPixelData.Create(fileAnon.Dataset);
                    var pixelData = PixelDataFactory.Create(header, header.NumberOfFrames - 1);

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

                        Dicom.IO.Buffer.MemoryByteBuffer buffer = new Dicom.IO.Buffer.MemoryByteBuffer(result);
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


                string sf = strPath + "/Anonymised/DICOM/" + strStudyID + ".dcm";

                int n = 1;
                try
                {

                    
                    fileAnon.Save(sf);   


                    /*while (n < 100)
                    {
                        sf = strPath + "/Anonymised/DICOM/" + strOriginalPatientID + "-" + n.ToString("00") + Path.GetFileName(strFile).Substring(0, 2) + ".dcm";
                        if(!File.Exists(sf))
                        {
                            break;
                        }
                        n++;
                    }
                    //fileAnon.Save(strPath + "/Anonymised/DICOM/" + i + ".dcm");
                    fileAnon.Save(sf);*/
                }
                catch (Exception ex)
                {
                    // Transmit message back with worker?
                    lFailed.Add(new Tuple<string, string, string>(strFile, "Header non-standard/corrupt", "\"" + ex.Message.Replace("\"", "\"\"") + "\""));
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
                    lFailed.Add(new Tuple<string, string, string>(strFile, "No image data in the DICOM file", "\"" + ex.Message.Replace("\"", "\"\"") + "\""));
                    System.Diagnostics.Debug.WriteLine(ex.ToString());

                    continue;
                }



                // Convert DCM to a 32-bit per pixel (8-bit per each color RGB + 8-bit unused) PNG file
                try
                {
                    Dicom.IO.PinnedIntArray px = img.RenderImage().Pixels;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Enabled == false)
            {
                return;
            }

            buttonAnon.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;

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
            backgroundWorker2.RunWorkerAsync(param);
        }

        // This event handler is where the actual,
        // potentially time-consuming work is done.
        private void backgroundWorker2_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = DatabaseFunc((Tuple<string, List<string>>)e.Argument, worker, e);
        }

        string DatabaseFunc(Tuple<string, List<string>> param, BackgroundWorker worker, DoWorkEventArgs e)
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

                if(file == null)
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
                catch(Exception ex)
                {
                    strAnonymizable = "No: " + ex.Message;
                }

                if(fileAnon != null)
                {
                    try
                    {
                        var header = DicomPixelData.Create(fileAnon.Dataset);
                        var pixelData = PixelDataFactory.Create(header, header.NumberOfFrames - 1);
                        strNonStandardImage = "Standard";
                    }
                    catch(Exception ex)
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

                    if(hashFieldCount.ContainsKey(strColumn))
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

                foreach(string strCol in lFieldsSorted)
                {
                    strColumns += "," + CSVEscape(strCol);
                }

                file.WriteLine(strColumns);
                foreach (Dictionary<string, string> dRow in lDataRaw)
                {
                    string strRow = CSVEscape(dRow["FileName"]) + "," + CSVEscape(dRow["InternalTransferSyntax"]) + "," + CSVEscape(dRow["Anonymizable"]) + "," + CSVEscape(dRow["NonStandardImage"]);

                    foreach(string strCol in lFieldsSorted)
                    {
                        if(dRow.ContainsKey(strCol))
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

        static bool GetDicomTagString(ref DicomFile file, DicomTag tag, out string value)
        {
            value = "";

            try
            {
                value = file.Dataset.GetValue<string>(tag, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Enabled == false)
            {
                return;
            }

            using (FormPatientID frm = new FormPatientID())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    buttonAnon.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;

                    richTextBoxOut.Clear();
                    richTextBoxOut.AppendText("Creating new DICOM files with reassigned PatientIDs using " + frm.strType);
                    progressBar.Minimum = 0;
                    progressBar.Maximum = 100;
                    progressBar.Value = 0;

                    try
                    {
                        if (Directory.Exists(_strFolder + "\\Recoded"))
                        {
                            richTextBoxOut.AppendText("\nCleaning up existing subfolder 'Recoded'.");
                            DirectoryInfo di = new DirectoryInfo(_strFolder + "\\Recoded");
                            di.Delete(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        richTextBoxOut.AppendText("\nSubfolder 'Recoded' could not be cleaned up.\n" + ex.Message);
                        buttonAnon.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = true;
                        return;
                    }

                    Directory.CreateDirectory(_strFolder + "\\Recoded");
                    richTextBoxOut.AppendText("\nSubfolder 'Recoded' successfully created.");

                    Tuple<string, string, List<string>> param = new Tuple<string, string, List<string>>(_strFolder, frm.strType, _lFiles);

                    // Start the asynchronous operation.
                    backgroundWorker3.RunWorkerAsync(param);
                }
            }
        }

        private void backgroundWorker3_DoWork(object sender,
            DoWorkEventArgs e)
        {
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            // Assign the result of the computation
            // to the Result property of the DoWorkEventArgs
            // object. This is will be available to the 
            // RunWorkerCompleted eventhandler.
            e.Result = WorkerFuncRecodePatientID((Tuple<string, string, List<string>>)e.Argument, worker, e);
        }

        string WorkerFuncRecodePatientID(Tuple<string, string, List<string>> param, BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<string> lFailed = new List<string>();
            string strPath = param.Item1;
            string strRecodeType = param.Item2;
            List<string> files = param.Item3;

            //List<Tuple<string, string, string, string>> listDCM = new List<Tuple<string, string, string, string>>();

            int i = 0, nRecodeValue = 1;

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

                string strOriginalPatientID = "";

                try
                {
                    strOriginalPatientID = file.Dataset.GetValue<string>(DicomTag.PatientID, 0);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }


                DicomTag[] tagsToRemove = { DicomTag.PatientID };

                foreach (DicomTag d in tagsToRemove)
                {
                    try
                    {
                        file.Dataset.Remove(d);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error removing element: " + ex.ToString());
                    }
                }

                try
                {
                    if (strRecodeType == "sequential numbering")
                    {
                        file.Dataset.Add(DicomTag.PatientID, nRecodeValue.ToString());
                        nRecodeValue++;
                    }
                    else
                    {
                        file.Dataset.Add(DicomTag.PatientID, Path.GetFileNameWithoutExtension(strFile));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error recoding PatientID: " + ex.ToString());
                }


                file.Save(strPath + "/Recoded/" + Path.GetFileName(strFile));


                worker.ReportProgress(i * 100 / files.Count);
                nSuccess++;

                //Console.WriteLine("Anonymized image " + i + " (of " + nFrames + " frame" + (nFrames == 1 ? "" : "s") + "): " + strFile);
            }


            string strRet = nSuccess.ToString() + " images successfully recoded.\nOutput at:\n" + strPath + "\\Recoded";
            if (lFailed.Count > 0)
            {
                strRet += "\nThese files failed to recode:";
                foreach (string sf in lFailed)
                {
                    strRet += "\n" + sf;
                }
            }
            return strRet;
        }

        private string CSVEscape(string str)
        {
            return "\"" + str.Replace("\"", "\"\"") + "\"";
        }
    }
}

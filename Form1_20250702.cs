﻿using Genetec;
using Genetec.Configuration.Core;
using Genetec.Interop.PInvoke;
using Genetec.Network;
using Genetec.Sdk;
using Genetec.Sdk.Entities;
using Genetec.Sdk.Entities.Behaviors;
using Genetec.Sdk.Entities.Video;
using Genetec.Sdk.Events.VideoAnalytics;
using Genetec.Sdk.Queries;
using Genetec.Sdk.Workflows;
using Genetec.Sdk.Workflows.UnitManager;
//using SdnCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsAddVideoUnit
{
    public partial class Form1 : Form
    {
        string path = string.Empty;
        //CommonClass sdn = new("WinFormsAddVideoUnit.ini");

        public Form1()
        {
            InitializeComponent();
            richTextBox.Enabled = false; //リッチテキストボックスを無効化
        }

        void btn_CSV_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.Title = "CSVファイルを選択してください。";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                textBox_CSV.Text = path;
            }
        }

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void btn_Execute_Click(object sender, EventArgs e)
        {
            btn_Execute.Enabled = false; //ボタン無効化
            btn_CSV.Enabled = false; //ボタン無効化
            textBox_CSV.Enabled = false; //テキストボックス無効化

            //全体のtry
            string myMethod = this.Name;
            string lastMsg = "";
            string lastSumMsg = "";
            int colorInt = 0;
            DialogResult finalDialog;
            int errorInt = 0; //エラーカウンタ

            try
            {
                DateTime dateTime = DateTime.Now;
                //string path = sdn.SYSCONFIG_PATH; //フルパス
                //sdn.SetInitLog();//Config.iniの設定を読み込む

                //const int LV_ADMIN = (int)SdnCommon.CommonClass.LOG_L.LV_ADMIN;
                //SdkResolver.Initialize(); //■Q2.Is this necessary?

                LogandTextBox("StartApp", $"=== GenetecSecurityCenter_AutoAddVideoUnit_StartTime：{dateTime} ==========", 0);

                ////Config.iniから各情報を取得
                //string IP = sdn.GetProfileStr("DIRECTORY", "IP", null, path);
                //string USERNAME = sdn.GetProfileStr("DIRECTORY", "USERNAME", null, path);
                //string PASSWORD = sdn.GetProfileStr("DIRECTORY", "PASSWORD", null, path);
                //string CERTIFICATE = sdn.GetProfileStr("DIRECTORY", "CERTIFICATE", null, path);
                //string ARCHIVER = sdn.GetProfileStr("DIRECTORY", "ARCHIVER", null, path);

                //■Please change it according to your test environment.
                string IP = "172.16.15.100";
                string USERNAME = "admin";
                string PASSWORD = "Sdn1015!";
                string CERTIFICATE = "KxsD11z743Hf5Gq9mv3+5ekxzemlCiUXkTFY5ba1NOGcLCmGstt2n0zYE9NsNimv";
                string ARCHIVER = "Archiver";


                //情報がどれか一つでもNULLの場合はエラーを表示
                string nullContent = "";
                int nullint = 0; //NULLチェックカウンタ

                ////NULLチェック
                //if (IP == null)
                //{
                //    IP = "NULL";
                //    nullContent += "[IP]";
                //    nullint++;
                //}
                //if (USERNAME == null)
                //{
                //    USERNAME = "NULL";
                //    nullContent += "[USERNAME]";
                //    nullint++;
                //}
                //if (PASSWORD == null)
                //{
                //    PASSWORD = "NULL";
                //    nullContent += "[PASSWORD]";
                //    nullint++;
                //}
                //if (CERTIFICATE == null)
                //{
                //    CERTIFICATE = "NULL";
                //    nullContent += "[CERTIFICATE]";
                //    nullint++;
                //}
                //if (ARCHIVER == null)
                //{
                //    ARCHIVER = "NULL";
                //    nullContent += "[ARCHIVER]";
                //    nullint++;
                //}


                //各情報の表示
                LogandTextBox("InfoCheck", "--- 以下のDirectoryとArchiverに接続します ---", 0);
                LogandTextBox("InfoCheck", $"IP : {IP}", 0);
                LogandTextBox("InfoCheck", $"USERNAME : {USERNAME}", 0);
                LogandTextBox("InfoCheck", $"PASSWORD : {PASSWORD}", 0);
                LogandTextBox("InfoCheck", $"CERTIFICATE : {CERTIFICATE}", 0);
                LogandTextBox("InfoCheck", $"ARCHIVER : {ARCHIVER}", 0);
                LogandTextBox("InfoCheck", "---------------------------------------------", 0);

                //if (nullint != 0)
                //{
                //    lastMsg += "【警告】 Config.ini > DIRECTORY > " + nullContent + " が空欄です。";
                //    colorInt = 1;
                //    //LogandTextBox("", lastMsg, 1);
                //    return;
                //}

                DialogResult dialogResult = MessageBox.Show("接続情報が正しいことを確認してください。続行する場合は「はい(Y)」、中断する場合は「いいえ(N)」を押下してください。"
                    , "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                    LogandTextBox("InfoCheck", "処理を続行します。", 0);
                else
                {
                    lastMsg = "中断します。";
                    //LogandTextBox("InfoCheck", lastMsg, 0);
                    return;
                }

                //Ping実行
                Ping ping = new Ping();
                var pingReply = ping.Send(IP); //Pingのタイムアウトを5秒に設定
                LogandTextBox("Ping", $"PingReplyStatus : {pingReply.Status}", 0);
                if (pingReply.Status != IPStatus.Success)
                {
                    lastMsg = "【警告】Ping失敗。Config.ini > DIRECTORY > [IP] を確認してください。";
                    colorInt = 1;
                    //LogandTextBox("Ping", lastMsg, 1);
                    return;
                }
                else
                    LogandTextBox("Ping", $"Ping成功！", 0);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //ログオン開始
                LogandTextBox("Logon", "ログオン開始", 0);
                using var engine = new Engine();

                engine.ClientCertificate = CERTIFICATE;
                ConnectionStateCode state = await engine.LogOnAsync(server: IP, username: USERNAME, password: PASSWORD);

                //ログオン不可の場合は終了
                if (state != ConnectionStateCode.Success)
                {
                    string stateStr = state.ToString();
                    colorInt = 1;
                    LogandTextBox("Logon", $"Logon failed : {state}", colorInt);
                    if (stateStr == "InvalidCredential")
                        lastMsg = $"【警告】ログオン失敗。Config.ini > DIRECTORY > [USERNAME],[PASSWORD]を確認してください。";
                    else if (stateStr == "CertificateRegistrationError")
                        lastMsg = $"【警告】ログオン失敗。Config.ini > DIRECTORY > [CERTIFICATE]を確認してください。";
                    else
                        lastMsg = $"【警告】ログオン失敗。";
                    return;
                }

                //ログオン有効の場合
                stopwatch.Stop(); //ストップウォッチ停止
                TimeSpan ts = stopwatch.Elapsed;
                LogandTextBox("Logon", $"ログオン成功！ Time : {ts.TotalSeconds}秒", 0);

                //ArchiverRole取得
                LogandTextBox("GetArchiver", "ArchiverRole取得開始", 0);
                ArchiverRole archiver = null;
                var queryArchiver = (EntityConfigurationQuery)engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration);
                queryArchiver.EntityTypeFilter.Add(EntityType.Role);
                var resultArchiver = await Task.Factory.FromAsync(queryArchiver.BeginQuery, queryArchiver.EndQuery, null);
                var archiverRoles = resultArchiver.Data.AsEnumerable().Select(row => engine.GetEntity(row.Field<Guid>(nameof(Guid)))).OfType<ArchiverRole>();

                foreach (var item in archiverRoles)
                {
                    string itemName = item.Name;

                    if (itemName == ARCHIVER)
                    {
                        archiver = item;
                        LogandTextBox("GetArchiver", $"ArchiverRole取得成功！ : {archiver.Name}", 0);
                        break;
                    }
                }
                if (archiver == null)
                {
                    lastMsg = "【警告】ArchiverRole取得失敗。Config.ini > DIRECTORY > [ARCHIVER]を確認してください。";
                    colorInt = 1;
                    return;
                }

                //Areaを全て取得
                LogandTextBox("GetAllArea", "全Area取得開始", 0);
                //Area area = null;
                var query = (EntityConfigurationQuery)engine.ReportManager.CreateReportQuery(ReportType.EntityConfiguration);
                query.EntityTypeFilter.Add(EntityType.Area);
                var result = await Task.Factory.FromAsync(query.BeginQuery, query.EndQuery, null);
                var AllAreas = result.Data.AsEnumerable().Select(row => engine.GetEntity(row.Field<Guid>(nameof(Guid)))).OfType<Area>();
                LogandTextBox("GetAllArea", "全Area取得終了！", 0);


                //CSVファイル読み込み
                LogandTextBox("ReadCSV", "CSV読込開始", 0);
                int csvRowCount = 0;
                List<string[]> csvData = new List<string[]>();
                try
                {
                    //string csvFilePath = path;
                    using (Microsoft.VisualBasic.FileIO.TextFieldParser textParser = new Microsoft.VisualBasic.FileIO.TextFieldParser(textBox_CSV.Text))
                    {
                        textParser.ReadLine(); //1行目Skip
                        textParser.SetDelimiters("|"); //Because "," is used in the product type, a different symbol is used.
                        while (!textParser.EndOfData)
                        {
                            string[] values = textParser.ReadFields();
                            csvData.Add(values);
                        }
                        textParser.Close();
                    }
                    csvRowCount = csvData.Count();
                    LogandTextBox("ReadCSV", $"CSV読込終了！ VideoUnit取込予定 : {csvRowCount}件", 0);
                }
                catch (Exception ex)
                {
                    lastMsg = $"【警告】CSV読込失敗。{ex.Message}";
                    colorInt = 1;
                    return;
                }
                int csvRow = 0;
                LogandTextBox("Loop", "--- VideoUnit取込開始 ---------------------------", 0);


                while (csvRow < csvRowCount)
                {
                    string VideoUnitName = csvData[csvRow][0];
                    string CamName1 = csvData[csvRow][1];
                    string CamName2 = csvData[csvRow][2];
                    string CamName3 = csvData[csvRow][3];
                    string CamName4 = csvData[csvRow][4];
                    string manufacturer = csvData[csvRow][5];
                    string producttype = csvData[csvRow][6];
                    string camUser = csvData[csvRow][7];
                    string camPwStr = csvData[csvRow][8];
                    string camIPStr = csvData[csvRow][9];
                    string camPortStr = csvData[csvRow][10];
                    string areaName = csvData[csvRow][11];
                    string stream = csvData[csvRow][12];
                    string streamLive = csvData[csvRow][13];
                    string streamArchiving = csvData[csvRow][14];
                    string streamRemote = csvData[csvRow][15];
                    string streamLow = csvData[csvRow][16];
                    string streamHigh = csvData[csvRow][17];
                    string resolution = csvData[csvRow][18];
                    string frameRate = csvData[csvRow][19];
                    string bitRate = csvData[csvRow][20];

                    csvRow++;
                    LogandTextBox("Loop", $"[ {csvRow} ]  Name : {VideoUnitName}", 0);

                    //Areaの取得
                    Area area = null;
                    foreach (var item in AllAreas)
                    {
                        string itemName = item.Name;
                        if (itemName == areaName)
                        {
                            area = item;
                            LogandTextBox("GetArea", $"Area : {area.Name}", 0);
                            break;
                        }
                    }

                    if (area == null)
                        LogandTextBox("GetArea", $"【警告】CSVで指定されたArea : {areaName} が見つかりません。Areaを指定せずにカメラを取り込みます。", 1);


                    //try位置変更　メーカー、製品タイプ不備もcatchできるように
                    try
                    {
                        //VideoUnitの各情報取得
                        SecureString camPW = camPwStr.ToSecureString();
                        IPAddress camIPAddress = IPAddress.Parse(camIPStr);
                        int camPort = int.Parse(camPortStr);

                        VideoUnitProductInfo productInfo = engine.VideoUnitManager.FindProductsByManufacturer(manufacturer).FirstOrDefault(p => p.ProductType == producttype);

                        if (productInfo == null)
                        {
                            LogandTextBox("GetProductInfo", $"Manufacturer : {manufacturer}・ProductType : {producttype} 誤字または組合せ不良。", 1);
                            throw new Exception("Manufacturer or ProductType does Not Exist.");
                        }

                        // Create the AddVideoUnitInfo object without duplicating IP/port
                        AddVideoUnitInfo addVideoUnitInfo = new(videoUnitProductInfo: productInfo,
                            ipEndPoint: new IPEndPoint(camIPAddress, camPort),
                            useDefaultCredentials: false);
                        addVideoUnitInfo.UserName = camUser;
                        addVideoUnitInfo.Password = camPW;

                        //VideoUnit追加
                        Guid areaGuid = area?.Guid ?? Guid.Empty; // Areaがnullの場合はGuid.Emptyを使用
                        Progress<EnrollmentResult> progress = new(result => LogandTextBox("AddVideoUnit", $"{result}", 0));
                        VideoUnitManager videoUnitManager = engine.VideoUnitManager;
                        Guid videoUnitId = await AddVideoUnit(videoUnitManager, addVideoUnitInfo, archiver.Guid, areaGuid, progress);

                        var videoUnit = (VideoUnit)engine.GetEntity(videoUnitId);
                        videoUnit.Name = VideoUnitName; //VideoUnit名変更

                        System.Collections.ObjectModel.ReadOnlyCollection<Guid> cameraList = videoUnit.Cameras;

                        int cameraInt = 0;

                        VideoCompressionAlgorithmType videoCompressionAlgorithm = VideoCompressionAlgorithmType.Unknown;
                        if (stream.Contains("H.264"))
                            videoCompressionAlgorithm = VideoCompressionAlgorithmType.H264;
                        else if (stream.Contains("H.265"))
                            videoCompressionAlgorithm = VideoCompressionAlgorithmType.HEVC; // H265はHEVCとして扱う???????
                        else if (stream.Contains("MJPEG"))
                            videoCompressionAlgorithm = VideoCompressionAlgorithmType.Mpeg2; //これでええんか


                        while (cameraInt < cameraList.Count())
                        {
                            Camera camera = (Camera)engine.GetEntity(cameraList[cameraInt]);
                            cameraInt++;

                            if (cameraInt == 1)
                                camera.Name = CamName1;
                            else if (cameraInt == 2)
                                camera.Name = CamName2;
                            else if (cameraInt == 3)
                                camera.Name = CamName3;
                            else if (cameraInt == 4)
                                camera.Name = CamName4;

                            LogandTextBox("AddVideoUnit", $"Firmware : {videoUnit.FirmwareVersion}", 0);
                            LogandTextBox("AddVideoUnit", $"Video unit : {videoUnit.Name} has been created.", 0);

                            //Create a dictionary using the number 1 in H.264-1 as the key.
                            Dictionary<int, VideoStream> CodecDictionary = camera.Streams.Select(engine.GetEntity)
                                .OfType<VideoStream>()
                                .Where(s => s.VideoCompressionAlgorithm == videoCompressionAlgorithm)
                                .ToDictionary(s => int.Parse(Regex.Match(s.Name, @"\d+$").Value), s => s);

                            SortedDictionary<int, VideoStream> sortedDictionary = new SortedDictionary<int, VideoStream>(CodecDictionary);

                            
                            bool SetCodecResult = false;
                            bool SetResolutionResult = false;

                            VideoStream EntityStream = null; //初期化

                            //Specify the codec and loop if there are multiple codecs
                            for (int i = 1; i <= sortedDictionary.Count; i++)
                            {
                                EntityStream = sortedDictionary[i]; 
                                LogandTextBox("AddVideoUnit", $"sortedDictionary[{i}] : {EntityStream.Name}", 0);
                                
                                camera.SetStreamUsage(StreamingType.Live, EntityStream.Guid); //Turn on the live stream.
                                Thread.Sleep(1500);

                                foreach (VideoStreamUsage streamUsage in camera.StreamUsages) //Loop through 5 StreamUsages.
                                {
                                    if (streamUsage.Usage == StreamingType.Live)
                                    {   
                                        LogandTextBox("AddVideoUnit", $"StreamUsage: {streamUsage.Usage.ToString()}", 0);
                                        var streamNowOn = (VideoStream)engine.GetEntity(streamUsage.Stream); //Get the stream that is currently on.

                                        LogandTextBox("AddVideoUnit", $"Stream: {streamNowOn.Name} | Codec: {streamNowOn.VideoCompressionAlgorithm}", 0);
                                        if (streamNowOn.Name == EntityStream.Name) //Check if the stream name matches the one we want to set.
                                        {
                                            SetCodecResult = true;
                                            LogandTextBox("AddVideoUnit", $"{EntityStream}へのLive設定成功。設定先：{streamNowOn.Name}", 0);
                                        }
                                        else
                                        {
                                            LogandTextBox("AddVideoUnit", $"{EntityStream}へのLive設定失敗。設定先：{streamNowOn.Name}", 0);
                                        }
                                        break; 
                                    }
                                }

                                if (SetCodecResult == true)
                                {
                                    SetResolutionResult = SetVideoResolution(EntityStream, camera, resolution);

                                    //解像度の設定が成功したか確認(?)
                                    if (SetResolutionResult == false)
                                        break; //解像度設定失敗したらループを抜ける（？）

                                    //ビットレートの設定 BitRate
                                    Thread.Sleep(2000);
                                    int bitRateInt = int.Parse(bitRate);
                                    EntityStream.SetBitRate(Schedule.AlwaysScheduleGuid, bitRateInt);
                                    EntityStream.SetMaximumAllowedBitRate(Schedule.AlwaysScheduleGuid, bitRateInt);
                                    LogandTextBox("VideoQuality", $"ビットレート : {bitRate}kbps", 0);
                                    LogandTextBox("VideoQuality", $"最大許容ビットレート : {bitRate}kbps", 0);


                                    //フレームレートの設定 FrameRate 
                                    Thread.Sleep(2000);
                                    int frameRateInt = int.Parse(frameRate);
                                    EntityStream.SetFrameRate(Schedule.AlwaysScheduleGuid, frameRateInt);
                                    LogandTextBox("VideoQuality", $"フレームレート : {frameRate}fps", 0);

                                    //設定後に確認する必要があるかも
                                    if (streamLive == "ON")
                                    {
                                        camera.SetStreamUsage(StreamingType.Live, EntityStream.Guid);
                                        LogandTextBox("VideoQuality", "ライブ : ON", 0);
                                    }
                                    if (streamArchiving == "ON")
                                    {
                                        camera.SetStreamUsage(StreamingType.Archiving, EntityStream.Guid);
                                        LogandTextBox("VideoQuality", "録画 : ON", 0);
                                    }
                                    if (streamRemote == "ON")
                                    {
                                        camera.SetStreamUsage(StreamingType.Remote, EntityStream.Guid);
                                        LogandTextBox("VideoQuality", "リモート : ON", 0);
                                    }
                                    if (streamLow == "ON")
                                    {
                                        camera.SetStreamUsage(StreamingType.LowQuality, EntityStream.Guid);
                                        LogandTextBox("VideoQuality", "低解像度 : ON", 0);
                                    }
                                    if (streamHigh == "ON")
                                    {
                                        camera.SetStreamUsage(StreamingType.HighQuality, EntityStream.Guid);
                                        LogandTextBox("VideoQuality", "高解像度 : ON", 0);
                                    }
                                    break; //streamListのループを抜ける。1つのカメラに対して1つのStreamを設定するため
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "Unable to enroll the video unit: BadLogin")
                            lastMsg = $"【取込失敗】{csvRow}台目「{VideoUnitName}」ログイン失敗。CSV File > User, Password を確認してください。";
                        else if (ex.Message == "Unable to enroll the video unit: Timeout")
                            lastMsg = $"【取込失敗】{csvRow}台目「{VideoUnitName}」ログイン失敗。CSV File > Manufacturer, ProductType, IP を確認してください。";
                        else if (ex.Message == "Unable to enroll the video unit: CantConnect")
                            lastMsg = $"【取込失敗】{csvRow}台目「{VideoUnitName}」ログイン失敗。CSV File > IP, Port を確認してください。";
                        else if (ex.Message == "Manufacturer or ProductType does Not Exist.")
                            lastMsg = $"【取込失敗】{csvRow}台目「{VideoUnitName}」メーカー・製品タイプ不明。CSV File > Manufacturer, Product type を確認してください。";

                        //else if (ex.Message == "Unable to enroll the video unit: CantConnect")
                        //    lastMsg = $"【警告】{csvRow}台目「{camName}」ログイン失敗。CSV File > Port を確認してください。";
                        else
                            lastMsg = ex.Message;

                        LogandTextBox("Exception", lastMsg, 1);
                        errorInt++;
                    }
                }
                //CSVの行数分ループ

                lastMsg = $"取込 {csvRow} 件。エラー {errorInt} 件。アプリを終了します。";
                lastSumMsg = $"取込 計 {csvRow} 件" + Environment.NewLine + $"エラー 計 {errorInt} 件" + Environment.NewLine + "アプリを終了します。";
                colorInt = 0;
            }
            catch (Exception ex)
            {
                LogandTextBox("Exception", ex.Message, 1);
            }
            finally
            {
                btn_Execute.Enabled = true; //ボタン有効化
                btn_CSV.Enabled = true; //ボタン有効化
                textBox_CSV.Enabled = true; //テキストボックス有効化
                LogandTextBox(myMethod, lastMsg, colorInt);

                if (colorInt == 0) finalDialog = MessageBox.Show(lastSumMsg, "アプリ終了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else finalDialog = MessageBox.Show(lastSumMsg, "アプリ終了", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (finalDialog != DialogResult.None) System.Windows.Forms.Application.Exit();
            }
        }


        bool SetVideoResolution(VideoStream EntityStream, Camera camera, string resolution)
        {
            bool setResolution = false;
            if (EntityStream is VideoStream)
            {
                //解像度の設定を先に実施する。希望の解像度が無い場合は、他のタブ？に移動する。
                Thread.Sleep(2000);
                char[] delimiter = { 'x', '×', '(', '（' }; //記号「×」でも分割できるようにする
                string[] requestedRes = resolution.Split(delimiter);

                int requestedWidth = int.Parse(requestedRes[0]);
                int requestedHeight = int.Parse(requestedRes[1]);

                VideoCompressionCapabilities capabilities = EntityStream.VideoCompressionCapabilities;

                var streamRes = capabilities.SupportedResolutions;

                foreach (StreamSupportedResolution res in streamRes) //現時点では完全一致でないと設定しない
                {
                    if (res.Width == requestedWidth && res.Height == requestedHeight)
                    {
                        EntityStream.SetResolution(Schedule.AlwaysScheduleGuid, res);
                        LogandTextBox("VideoQuality", $"解像度 : {res.Width}x{res.Height}", 0);
                        setResolution = true;
                        break;
                    }
                }
            }
            return setResolution;
        }

        void LogandTextBox(string myMethodName, string msgStr, int ColorInt)
        {
            //sdn.DoLogAsync(myMethodName, msgStr);

            if (ColorInt == 0)
                richTextBox.SelectionColor = Color.Black;
            else if (ColorInt == 1)
                richTextBox.SelectionColor = Color.Red;

            richTextBox.AppendText(msgStr + Environment.NewLine);
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        /// <summary>
        /// ビデオユニット追加
        /// </summary>
        /// <param name="videoUnitManager"></param>
        /// <param name="videoUnitInfo"></param>
        /// <param name="archiver"></param>
        /// <param name="area"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        async Task<Guid> AddVideoUnit(IVideoUnitManager videoUnitManager, AddVideoUnitInfo videoUnitInfo, Guid archiver, Guid area, IProgress<EnrollmentResult> progress = default)
        {
            var completion = new TaskCompletionSource<Guid>();

            videoUnitManager.EnrollmentStatusChanged += OnEnrollmentStatusChanged;
            try
            {
                AddUnitResponse response;
                if (area != Guid.Empty)
                    response = await videoUnitManager.AddVideoUnit(videoUnitInfo, archiver, area);
                else
                    response = await videoUnitManager.AddVideoUnit(videoUnitInfo, archiver);

                if (response.Error != Genetec.Sdk.Workflows.UnitManager.Error.None)
                    throw new Exception($"Fail to add video unit: {response.Error}");

                return await completion.Task;
            }
            finally
            {
                videoUnitManager.EnrollmentStatusChanged -= OnEnrollmentStatusChanged;
            }

            void OnEnrollmentStatusChanged(object sender, Genetec.Sdk.EventsArgs.UnitEnrolledEventArgs e)
            {
                progress?.Report(e.EnrollmentResult);

                if (e.EnrollmentResult != EnrollmentResult.Connecting)
                {
                    //Added an if statement. If AlreadyAdded, the loop will exit and the next step will be executed.
                    if (e.EnrollmentResult == EnrollmentResult.AlreadyAdded)
                        completion.SetException(new Exception($"The video unit is already added: {e.Unit}"));
                    else if (e.Unit != Guid.Empty)
                        completion.SetResult(e.Unit);
                    else
                        completion.SetException(new Exception($"Unable to enroll the video unit: {e.EnrollmentResult}"));
                }
            }
        }
    }
}

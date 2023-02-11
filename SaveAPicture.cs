using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace アプリ
{
    class SaveAPicture
    {
        //画像をNASに保存する
        //それに、下記の機能を含めている。
        //①保存しようとしている画像の生成時間のHourが変わった場合、
        //新しいフォルダーを生成して、その時間帯の画像を保存する。
        public static void copyAPic_NewFolderPerHour(string cameraPicFolder, string destinationFolder, string picName, string picAfterCopyName, DateTime picCreationTime, string fileType)
        {

            //保存しようとする画像のパスを作成  
            string picToBeCopied = cameraPicFolder + "\\" + picName;
            string picPathInDestination = "";

            //Retry  when error occurs during compression.
            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
            {
                try
                {

                    //If the picture has been created,
                    //copy the picture to NAS.
                    if (File.Exists(picToBeCopied))
                    {
                        //[Step1 NAS上の保存先を確保する]
                        //Get the final destination folder for the current picture.
                        //ケース1　フォルダーの作成が必要ない場合、デフォルトフォルダーパスを返す。
                        //ケース2　フォルダーの作成が必要ない場合、
                        //フォルダーの作成が成功した場合、そのフォルダーのパスを返す。
                        //失敗した場合、"新しいフォルダー作成失敗"の文字を返す。
                        string finalDestinationFolder = CreateFolders.createNewFolder_IfPicCreationTimeHourChange(destinationFolder, picCreationTime, ref GlobalConstants.lastSavedOriginalPicMonth, ref GlobalConstants.lastSavedOriginalPicDay, ref GlobalConstants.lastSavedOriginalPicHour);

                        if (finalDestinationFolder == "新しいフォルダー作成失敗")
                        {
                            //Give up saving this picture.
                            return;
                        }



                        //[Step2 オリジナル画像の保存]
                        //Rename the picture with its creation time.
                        picPathInDestination = finalDestinationFolder + "\\" + picAfterCopyName;
                        //Copy the picture to NAS
                        File.Copy(picToBeCopied, picPathInDestination);
                        return;
                    }
                }

                //If for some abnormal reason, the picture disappears when we are saving it,
                //we give up saving the picture to NAS without retry,and output an error message.
                catch (FileNotFoundException e)
                {


                    string errorMessage = "";
                    //display this error message to inform the user
                    errorMessage = "一枚の" + fileType + ": " + picToBeCopied + " は、\n"+
                        "NASに保存する途中でエラーが発生しました。\nこの画像が存在していないので、\nこの画像をNASに保存するのを諦めて、次の画像を保存する。\n\n" + "エラーメッセージ：\n" + e.Message;

                    //output the error message  
                    //to the DataManagementApp_エラーメッセージ folder in NAS
                    ReportErrorMsg.outputErrorMsg(fileType + "をNASに保存", errorMessage);

                    //wait a while to let the program output the error txt message
                    //error message can not be output without this wait 
                    Thread.Sleep(1000);

                    //Do not retry for this exception
                    //Give up saving this picture.
                    return;
                }


                //Retry for 3 times and output error message  
                //for the rest of the exceptions which happen while copying pictures to NAS
                catch (Exception e)
                {

                    // If it's still within retry times limit
                    if (retryTimes < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {

                        //display this error message to inform the user
                        string errorMessage = "一枚の"+fileType +": "+ picToBeCopied + " は、\n" +
                            "NASに保存する途中でエラーが発生しました。\n今回は" + GlobalConstants.retryTimesLimit + "回目のRetryです。\nRetry回数の上限に達しましたので、\nこの画像をNASに保存するのを諦めて、次の画像を保存する。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //show a pop-up message to inform the operator that if this error
                        //occurs so many times, contact 管理者 for help.
                        ReportErrorMsg.showMsgBox_IfNotShown("一枚の"+fileType+"は、\n"+ GlobalConstants.retryTimesLimit + "回RetryしてもNASに保存できない。\nその画像を保存するのを諦めて、次の画像を保存する。\n\nこのエラー何回も発生した場合、\n停止ボタンを押してを停止して、\n管理者に連絡してください。" + "\n\n管理者への解決手順：\nStep1 NASへの接続とNASの状態を確認してください。\nStep2 システムを再起動して、このエラーが長い時間で一回だけ発生した場合、単純に一時的なNASへの接続不具合だからです。\n多発している場合、画像をNASに保存する機能の修正が必要となります。" + "\n\nこのエラーの原因：\n" + e.Message, " " + GlobalConstants.PIInspectTarget + "_"+fileType+"をNASに保存できないエラー");

                        //output the error message 
                        //to the DataManagementApp_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(fileType+"をNASに保存", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                    }
                }
            }

        }



        //Save the picture to NAS by using a ThreadPool Thread
        //成功した場合、trueを返す
        //失敗した場合、falseを返す
        public static async void copyAPicAsync(string picPath, string destinationFolder, string picAfterCopyName)
        {
            //Collect the first N problem images to NAS　for each HALCON error(e.g.,"画像を処理できない"、"ROI設置用ターゲットを認識できない") 
            try
            {
                
                //Create the folder for saving problem images if it doesn't exist.
                Directory.CreateDirectory(destinationFolder);

                //Async copy the problem image to NAS.
                using (FileStream SourceStream = File.Open(picPath, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(destinationFolder + "\\" + picAfterCopyName))
                    {
                        await SourceStream.CopyToAsync(DestinationStream);
                    }
                }

                

            }
            //Give up collecting the problem image to NAS if it fails.
            catch
            {
                return;
            }
        }

    }
}

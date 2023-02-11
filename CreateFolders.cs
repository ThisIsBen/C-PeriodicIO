using System;
using System.IO;
using System.Threading;

namespace App
{
    class CreateFolders
    {

        //保存する画像の生成時間に変換があれば、NAS上に新しいフォルダを作成して画像を保存する。
        //ケース1　フォルダーの作成が必要ない場合、デフォルトフォルダーパスを返す。
        //ケース2　フォルダーの作成が必要ない場合、
        //フォルダーの作成が成功した場合、そのフォルダーのパスを返す。
        //失敗した場合、"新しいフォルダー作成失敗"の文字を返す。
        public static string createNewFolder_IfPicCreationTimeHourChange(string cameraNASEachStart_FolderPath, DateTime picCreationTime, ref int currentCameraNASMonthRecorder, ref int currentCameraNASDayRecorder, ref int currentCameraNASHourRecorder)
        {
            //set the default NAS保存先 of the current camera
            string finalDestinationFolder = cameraNASEachStart_FolderPath + "\\" + currentCameraNASMonthRecorder.ToString("D2") + "_" + currentCameraNASDayRecorder.ToString("D2") + "\\" + currentCameraNASHourRecorder.ToString("D2");


            #region 保存する画像の生成時間に変換があれば、NAS上に新しいフォルダを生成して画像を保存する。
            if (currentCameraNASHourRecorder != picCreationTime.Hour || currentCameraNASDayRecorder != picCreationTime.Day || currentCameraNASMonthRecorder != picCreationTime.Month)
            {
                //Update currentCameraNASMonthRecorder and currentCameraNASDayRecorder and currentCameraNASHourRecorder
                currentCameraNASMonthRecorder = picCreationTime.Month;
                currentCameraNASDayRecorder = picCreationTime.Day;
                currentCameraNASHourRecorder = picCreationTime.Hour;


                //新しい現場確認用画像保存フォルダーパスを作る
                finalDestinationFolder = cameraNASEachStart_FolderPath + "\\" + currentCameraNASMonthRecorder.ToString("D2") + "_" + currentCameraNASDayRecorder.ToString("D2") + "\\" + currentCameraNASHourRecorder.ToString("D2");



                //Create the new folder on NAS.
                //フォルダーの生成が失敗した場合、
                //新しい前後の画像保存フォルダーパスを"前後の画像保存フォルダー生成失敗"にする。
                if (createFolderWithRetry(finalDestinationFolder, "前後の画像保存フォルダー") == false)
                {
                    finalDestinationFolder = "新しいフォルダー作成失敗";
                }




            }
            #endregion

            return finalDestinationFolder;

        }

        //指定されたフォルダーを作成し、
        //作成が成功した場合、trueを返す。
        //失敗した場合、falseを返す。
        public static bool createFolderWithRetry(string folderPath, string folderType)
        {
            //Retry  when error occurs during compression.
            for (int retryTimes_CreateDir = 1; retryTimes_CreateDir <= GlobalConstants.retryTimesLimit; retryTimes_CreateDir++)
            {

                try
                {
                    //Create a new folder 
                    Directory.CreateDirectory(folderPath);
                    return true;
                }
                catch (Exception e)
                {
                    string errorMessage = "";
                    // If it's still within retry times limit
                    if (retryTimes_CreateDir < GlobalConstants.retryTimesLimit)
                    {
                        //wait a while before starting next retry
                        Thread.Sleep(GlobalConstants.retryTimeInterval);
                    }

                    //If it has reached the retry limit
                    else
                    {

                        //output this error message to inform the user
                        errorMessage = folderPath + "に"+ folderType+ "をNAS上に作っている途中でエラーが発生しました。\n今回は " + retryTimes_CreateDir + "回目のRetryです。\nRetry回数の上限に達しましたので、Retryしません。\n\n前後の画像をNASに保存する機能が自動的に停止した。\n\n" + "停止ボタンを押してを停止して、\n管理者に連絡してください。" + "\n\n管理者への解決手順：\nStep1 NASへの接続とNASの状態を確認してください。\nStep2 Step 1は問題がない場合、手動でNAS上にフォルダーを生成できるかどうかを確認してください。\nStep3 Step2 も問題がない場合、画像保存フォルダをNASに生成する機能の修正が必要となります。" + "\n\nこのエラーの原因：\n" + e.Message;

                        //show a pop-up message to inform the operator that if this error
                        //occurs so many times, contact 管理者 for help.
                        ReportErrorMsg.showMsgBox_Anyway(errorMessage, " " + GlobalConstants.TCPSocketServerName + folderType+"をNASに生成できないエラー");

                        //output the error message to                            
                        //the DataManagementApp_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(folderType+"をNASに生成できない", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);
                        
                    }
                }

            }

            //Give up saving images to NAS
            //and end this function becasue we have reached the limit of retry
            //NAS上の保存先作成できないため、今の画像の保存を諦めた。
            return false;
        }




    }
}

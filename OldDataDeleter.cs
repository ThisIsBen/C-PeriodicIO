using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    //古いデータ自動削除機能:
    //定期的に指定したフォルダ内の更新時間が時間閾値より前の古いフォルダとファイルを削除する
    class OldDataDeleter
    {


        //古いデータ自動削除機能の対象名　例えば、"現場確認用オリジナル画像"
        private string delDataType;

        //古いデータ自動削除機能の対象フォルダ
        private string[] targetFolders;

        //古いデータ自動削除機能の時間閾値
        //閾値より前のデータを削除する
        private uint HourThreshold;

        //一回当たりの削除ファイル数(単位：個)。
        //制限なしは0に設定する。
        private uint delFileCountLimit;

        //一つファイルを削除した後に待つ削除間隔(単位：秒)。
        //制限なしは0に設定する。
        private int pauseAfterEachDel;





        //削除の方式を選ぶ："一回だけ"もしくは"定期的に"
        private string DeleteWay; 

        //1回目の削除が実施される前に待たせたい時間
        private int timerDelayTime;

        //削除の実施周期を記録する。
        private uint timerInterval;

        //定期的に指定したフォルダ内の更新時間が時間閾値より前の古いフォルダとファイルを削除するためのTimerを宣言する
        private Timer delOldDataTimer;

        #region 削除対象フォルダが変わる時に更新すべき変数
        //削除対象フォルダを削除させないように、削除対象フォルダのパスを記録する。
        private string rootFolder;
        //一回当たりの削除ファイル数に制限があるため、削除したファイル数をカウントする。
        private uint delFileCount = 0;
        #endregion





        /// <summary>
        /// 設定した最後更新日時より古いデータを削除する機能の初期化。
        /// </summary>
        /// <param name="arg_delDataType"></param>削除するデータの名称。例：前後の画像
        /// <param name="arg_targetFolders"></param>削除の対象フォルダパス
        /// <param name="arg_HourThreshold"></param>古いデータの判定基準（単位：時間）。時間閾値 = 0の場合、削除しない。
        /// 
        /// <param name="arg_delFileCountLimit"></param>一回当たりの削除ファイル数(単位：個)。制限なしは0に設定する。
        /// <param name="arg_pauseAfterEachDel"></param>一つファイルを削除した後に待つ削除間隔(単位：秒)。制限なしは0に設定する。

        public OldDataDeleter(string arg_delDataType, string[] arg_targetFolders, uint arg_HourThreshold, uint arg_delFileCountLimit,int arg_pauseAfterEachDel)
        {
            delDataType = arg_delDataType;
            targetFolders = arg_targetFolders;
            HourThreshold = arg_HourThreshold;
            
            delFileCountLimit = arg_delFileCountLimit;
            pauseAfterEachDel = arg_pauseAfterEachDel;
            
        }






        /// <summary>
        /// 古いデータ自動削除機能を起動する
        /// </summary>
        /// <param name="arg_DeleteWay"></param>削除の方式を選ぶ："一回だけ"もしくは"定期的に"
        /// <param name="arg_timerDelayTime"></param>一回目の削除が始まる前の待ち時間
        /// <param name="arg_timerInterval"></param>削除の周期。（"定期的に"削除する場合のみ必要）

        //一回目の削除が始まる前に待たない場合、arg_timerDelayTimeがいらなく、
        //"一回だけ削除"したい場合、arg_timerIntervalがいらなく、
        //そのため、arg_timerDelayTimeとarg_timerIntervalはオプションのパラメータにして、
        //使われていない場合、arg_timerDelayTimeを0、arg_timerIntervalを1時間（3600＊1000）に初期化する。

        public void startDeleteOldData(string arg_DeleteWay, int arg_timerDelayTime = 0, uint arg_timerInterval = 3600 * 1000)
        {

            

            //更新時間が時間閾値より後のフォルダとファイルのみを残す。
            //時間閾値 = 0の場合、削除しない。
            if (HourThreshold > 0)
            {


                //削除の方式を取得
                DeleteWay = arg_DeleteWay;


                //"定期的に削除"したい場合、
                //1つのThreadPool Threadを 
                //Timerにより定期的に起動し、トラブル前後画像の過去の画像を削除
                if (DeleteWay == "periodical")
                {
                    timerDelayTime = arg_timerDelayTime;
                    timerInterval = arg_timerInterval;

                    //古いデータ自動削除機能に使うTimerを起動する。
                    //これで古いデータ自動削除機能が起動した。
                    delOldDataTimer = new Timer(
                    _ => deleteDataInEachSpecifiedFolder(),
                        null,
                    timerDelayTime,
                    timerInterval);
                }

                //"一回だけ削除"したい場合、
                //1つのThreadPool Threadを起動して、削除機能を実施する
                else if (DeleteWay == "once")
                {
                    timerDelayTime = arg_timerDelayTime;

                    Task.Run(() => {
                        //削除を実施する前にしばらく待たせたい場合、
                        //ThreadPool Threadを指定した時間を待たせる。
                        if (timerDelayTime>1)
                        {
                            Thread.Sleep(timerDelayTime);
                        }
                        
                        deleteDataInEachSpecifiedFolder(); 
                    }); 

                }

            }
        }



        //指定した複数のフォルダ内の更新時間が時間閾値より前のフォルダーとファイルを削除する。
        public void deleteDataInEachSpecifiedFolder()
        {
            //"定期的に削除"の場合、
            //If there are lots of old data to be deleted,
            //the deletion might not be finished before the next timer call.

            //To avoid overlapping timer calls,
            //we stop the timer before we start deleting old data,
            //and restart the timer after we finish deleting old data.

            //Stop the timer
            if (DeleteWay == "periodical")
            {
                delOldDataTimer.Change(-1, -1);
            }



            //Start deleting old data
            for (int i = 0; i < targetFolders.Length; i++)
            {
                //削除対象フォルダを削除させないように、
                //削除対象フォルダのパスを記録する。
                rootFolder = targetFolders[i];

                //削除を実施する
                deleteDataOlderThan_HourThreshold(targetFolders[i]);
            }



            //"定期的に削除"の場合、
            //Reset the timer after we finish deleting old data.
            if (DeleteWay == "periodical")
            {
                delOldDataTimer.Change(timerInterval, -1);
            }
        }



        public void deleteDataOlderThan_HourThreshold(string targetFolder)
        {
            //毎回削除するファイル数に制限がある場合、
            //削除したファイル数のカウンターをリセットする。
            if (delFileCountLimit > 0)
            {
                delFileCount = 0;
            }

            //削除を実施する
            privateMethodFor_DeleteOldData(targetFolder);
        }



        //指定したフォルダ内の更新時間が時間閾値より前のフォルダーとファイルを削除する。
        //初めてこの関数を実行する時に、delFileCountLimit > 0場合、delFileCount=0の初期化が必要。
        private void privateMethodFor_DeleteOldData(string targetFolder)
        {

            //Check whether the folder exists, if not, don't do anything
            if (Directory.Exists(targetFolder))
            {


                DirectoryInfo folder = new DirectoryInfo(targetFolder);
                
                try
                {
                    //Recursively go into subfolders to delete files inside them.
                    foreach (DirectoryInfo subfolder in folder.EnumerateDirectories())
                    {
                        //一回当たりの削除ファイル数制限がある場合
                        if (delFileCountLimit > 0)
                        {
                            //一回当たりの削除ファイル数が制限を超えた場合、
                            //returnする。
                            if (delFileCount >= delFileCountLimit)
                            {
                                return;
                            }
                            //一回当たりの削除ファイル数が制限以内の場合かつ、
                            //subfolderは生成時間が時間閾値より古い場合のみ、中のファイルの更新時間を確認する。
                            if (subfolder.CreationTime < DateTime.Now.AddHours(-1 * HourThreshold))
                            {
                                privateMethodFor_DeleteOldData(subfolder.FullName);
                            }
                        }
                        

                        //一回当たりの削除ファイル数制限がない場合               
                        else
                        {
                            //subfolderは生成時間が時間閾値より古い場合のみ、中のファイルの更新時間を確認する。
                            if (subfolder.CreationTime < DateTime.Now.AddHours(-1 * HourThreshold))
                            {
                                privateMethodFor_DeleteOldData(subfolder.FullName);
                            }
                        }


                    }

                



                    //一回当たりの削除ファイル数制限がある場合、
                    //設定枚数分の古いファイルを削除したら、削除作業を終了し、次の削除周期を待つ。
                    if (delFileCountLimit > 0)
                    {

                        //When there is no subfolder in the current folder,
                        //delete files older than time threshold(K_Hours).
                        foreach (FileInfo file in folder.EnumerateFiles())
                        {


                            //一回当たりの削除ファイル数が制限以内の場合、
                            //削除し続ける
                            if (delFileCount < delFileCountLimit)
                            {



                                //時間閾値より更新時間が古い画像を削除する。
                                if (file.LastWriteTime < DateTime.Now.AddHours(-1 * HourThreshold))
                                {
                                    //Delete a file
                                    try
                                    {

                                        file.Delete();

                                        //削除したファイル数をカウントする。
                                        delFileCount++;

                                        //削除間隔が必要な場合、指定された時間を待つ
                                        if (pauseAfterEachDel > 1)
                                        {
                                            Thread.Sleep(pauseAfterEachDel);
                                        }
                                    }

                                    //For the rest of the exceptions,
                                    //for example, "the specified file is in use",
                                    //we retry and output error message when the retry times limit is reached.
                                    catch (Exception e)
                                    {


                                        string errorMessage = "";

                                        //output this error message to inform the user
                                        errorMessage = delDataType + " を削除する途中でエラーが発生しました。" +
                                            "\n\nこのファイルを削除するのを諦めて、\n次の削除周期でもう一度削除することになる。" +
                                            "何回も発生した場合、管理者に連絡してください。\n" +
                                            "管理者への解決手順：\n\n" +
                                            "NASとの接続と設定ファイル内の" + delDataType + "のパスが合っているのかを確認してください。\n" +
                                                "エラーメッセージ：\n" + e.Message;

                                        //show the error message box to inform the user if the same message box is not being shown 
                                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, " " + GlobalConstants.TCPSocketServerName + "_" + delDataType + "削除エラー");

                                        //output the error message                         
                                        //to the DataManagementApp_エラーメッセージ folder in NAS
                                        ReportErrorMsg.outputErrorMsg(delDataType + "削除", errorMessage);

                                        //wait a while to let the program output the 3rd retry error txt message
                                        //The 3rd retry error message can not be output without this wait 
                                        Thread.Sleep(1000);


                                    }
                                }

                            }
                            //一回当たりの削除ファイル数が制限を超えた場合、
                            //削除をやめてreturnする。
                            else
                            {
                                return;
                            }

                        }



                    }

                    //一回当たりの削除ファイル数制限がない場合、
                    //すべての古いファイルを削除する。
                    else
                    {
                        //When there is no subfolder in the current folder,
                        //delete files older than time threshold(K_Hours).
                        foreach (FileInfo file in folder.EnumerateFiles())
                        {
                            //時間閾値より更新時間が古い画像を削除する。
                            if (file.LastWriteTime < DateTime.Now.AddHours(-1 * HourThreshold))
                            {
                                //Delete a file
                                try
                                {
                                    file.Delete();

                                    //削除間隔が必要な場合、指定された時間を待つ
                                    if (pauseAfterEachDel > 1)
                                    {
                                        Thread.Sleep(pauseAfterEachDel);
                                    }
                                }

                                //For the rest of the exceptions,
                                //for example, "the specified file is in use",
                                //we retry and output error message when the retry times limit is reached.
                                catch (Exception e)
                                {


                                    string errorMessage = "";

                                    //output this error message to inform the user
                                    errorMessage = delDataType + " を削除する途中でエラーが発生しました。" +
                                        "\n\nこのファイルを削除するのを諦めて、\n次の削除周期でもう一度削除することになる。" +
                                        "何回も発生した場合、管理者に連絡してください。\n" +
                                        "管理者への解決手順：\n\n" +
                                        "NASとの接続と設定ファイル内の" + delDataType + "のパスが合っているのかを確認してください。\n" +
                                            "エラーメッセージ：\n" + e.Message;

                                    //show the error message box to inform the user if the same message box is not being shown 
                                    ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, " " + GlobalConstants.TCPSocketServerName + "_" + delDataType + "削除エラー");

                                    //output the error message                         
                                    //to the DataManagementApp_エラーメッセージ folder in NAS
                                    ReportErrorMsg.outputErrorMsg(delDataType + "削除", errorMessage);

                                    //wait a while to let the program output the 3rd retry error txt message
                                    //The 3rd retry error message can not be output without this wait 
                                    Thread.Sleep(1000);


                                }
                            }
                        }
                    }


                }
                //古いファイルを削除中のフォルダ内のsubfolderやファイルを取得しようとする時に、
                //エラーが発生したら、このsubfolderの中身は次の削除周期で削除することになる。
                catch (Exception e)
                {
                    Console.WriteLine("古いファイルを削除中のフォルダ:\n" + targetFolder+ "内の" +
                   "subfolderやファイルを取得しようとする時に、下記のエラーが発生したため、\n" +
                   "このフォルダの中身は次の削除周期で確認することになる。\n" +
                   "エラーメッセージ：\n" + e.Message);

                    return;

                }



                //We don't delete the root folder even if it becomes empty,
                //and only delete the subfolders if no files are left inside them
                if (targetFolder != rootFolder)
                {
                    
                    //Delete the subfolder.
                    //The subfolder should be empty when reaching this point.
                    try
                    {
                        folder.Delete();
                    }
                    //If the subfolder is not empty,we do the following:

                    //If a user manually add new files to this old subfolder,
                    //the new files are not deleted,so the subfolder is not empty.
                    //We don't delete this subfolder this time.
                    //We delete this subfolder when those new files are old enough to be deleted,which mades this subfolder empty.
                    catch (IOException)
                    {
                        return;
                    }

                    //For the rest of the exceptions,
                    //for example, "the specified file is in use",
                    //we retry and output error message when the retry times limit is reached.
                    catch (Exception e)
                    {


                        string errorMessage = "";

                        //output this error message to inform the user
                        errorMessage = delDataType + " を削除する途中でエラーが発生しました。" +
                            "\n\nこのフォルダーを削除するのを諦めて、\n次の削除周期でもう一度削除することになる。" +
                            "何回も発生した場合、管理者に連絡してください。\n" +
                            "管理者への解決手順：\n\n" +
                            "NASとの接続と設定ファイル内の" + delDataType + "のパスが合っているのかを確認してください。\n" +
                                "エラーメッセージ：\n" + e.Message;

                        //show the error message box to inform the user if the same message box is not being shown 
                        ReportErrorMsg.showMsgBox_IfNotShown(errorMessage, " " + GlobalConstants.TCPSocketServerName + "_" + delDataType + "削除エラー");

                        //output the error message                         
                        //to the DataManagementApp_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg(delDataType + "削除", errorMessage);

                        //wait a while to let the program output the 3rd retry error txt message
                        //The 3rd retry error message can not be output without this wait 
                        Thread.Sleep(1000);

                    }
                    
                }

            }


        }




    }
}

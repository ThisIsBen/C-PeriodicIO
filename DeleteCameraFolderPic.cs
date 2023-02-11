using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class DeleteCameraFolderPic
    {

        

        //the camera folder path that the current thread
        //is dealing with
        private string cameraPicFolderPath;

        //the DeleteList for the current thread
        private List<string> currentDeleteList;
        //To specify which DeleteList_Lock to use to protect the currentDeleteList
        //from being accessed by different thread at the same time.
        private int DeleteList_LockIndex;

        //the SaveList for the current thread
        private List<string> currentSaveList;

        //to indicate which camera that the current thread is dealing with
        private int cameraNoOfThisThread;


       

        //DeleteListの初期化
        public DeleteCameraFolderPic(int cameraNo)
        {

            //Initiate DeleteList_LockIndex
            DeleteList_LockIndex = cameraNo - 1;

            //Record which camera that the current thread is dealing with
            cameraNoOfThisThread = cameraNo;

            //Assign the cameraPicFolder, KeepList,DeleteList for this thread according to which camera the thread is dealing with
            switch (cameraNo)
            {
                case 1:
                    {
                        cameraPicFolderPath = GlobalConstants.camera1PicFolderPath;
                        currentDeleteList = GlobalConstants.camera1DeleteList;
                        currentSaveList= GlobalConstants.camera1SaveList;
                        break;
                    }
                case 2:
                    {
                        cameraPicFolderPath = GlobalConstants.camera2PicFolderPath;
                        currentDeleteList = GlobalConstants.camera2DeleteList;
                        currentSaveList = GlobalConstants.camera2SaveList;
                        break;
                    }

                case 3:
                    {
                        cameraPicFolderPath = GlobalConstants.camera3PicFolderPath;
                        currentDeleteList = GlobalConstants.camera3DeleteList;
                        currentSaveList = GlobalConstants.camera3SaveList;
                        break;
                    }
                case 4:
                    {
                        cameraPicFolderPath = GlobalConstants.camera4PicFolderPath;
                        currentDeleteList = GlobalConstants.camera4DeleteList;
                        currentSaveList = GlobalConstants.camera4SaveList;
                        break;
                    }
                case 5:
                    {
                        cameraPicFolderPath = GlobalConstants.camera5PicFolderPath;
                        currentDeleteList = GlobalConstants.camera5DeleteList;
                        currentSaveList = GlobalConstants.camera5SaveList;
                        break;
                    }
                case 6:
                    {
                        cameraPicFolderPath = GlobalConstants.camera6PicFolderPath;
                        currentDeleteList = GlobalConstants.camera6DeleteList;
                        currentSaveList = GlobalConstants.camera6SaveList;
                        break;
                    }


            }
           


        }




        //The task for the deleteOldPicThread:
        //Task ①定期的にDeleteList内の画像とそれらの画像に該当する情報ファイルを全部削除する。

        //Task ②画像を削除する前にその画像がSaveListに存在しているかどうかを確認する。
        //存在している場合、今回削除をやめて、次の周期で削除する。 

        //Task ③SaveList内の画像数が設定された閾値を超えた場合、
        //「NASに画像保存進捗非常に遅い」とみなす。
        //pop-upメッセージでエラーメッセージを表示し、
        //画像を削除する前にその画像がSaveListに存在しているかどうかを確認することをやめて、
        //無条件で画像を直接削除する。
        public void deleteOldPicTask()
        {
            //to get the number of pictures in the DeleteList
            int deleteListCount_BeforeDeletion;


            //DeleteList中の画像のパスを作る
            string picToBeDeleteFilePath;


            //削除された画像の情報ファイルのパスを作る
            string infoFileToBeDeleteFilePath;




            





            //Task ② 
            //画像を削除する前にその画像がSaveListに存在しているかどうかを確認する。
            //存在している場合、今回削除をやめて、次の周期で削除する。
            #region Make sure the DeleteList will not be cleared when "the picture to be deleted is being saved to NAS at the same time" happens
            //to indicate if "the picture to be deleted is being saved to NAS at the same time" happened
            bool picHasNotBeenSaved = false;

            //to record the index of the picture in the DeleteList that is to be deleted and saved to NAS at the same time
            int picDeleteSaveAtSameTime_Index = 0;
            #endregion





            //Task ③
            //SaveList内の画像数が設定された閾値を超えた場合、
            //「NASに画像保存進捗非常に遅い」とみなす。
            //pop-upメッセージでエラーメッセージを表示し、
            //画像を削除する前にその画像がSaveListに存在しているかどうかを確認することをやめて、
            //無条件で画像を直接削除する。
            #region Check if 「NASに画像保存進捗非常に遅い」happened

            //to indicate whether "画像をNASに保存する機能" is out of order
            bool SaveToNASTooSlow_DeletePicNoMatterWhat = false;

            #endregion






            //Task ① 
            //定期的にDeleteList内の画像とそれらの画像に該当する情報ファイルを全部削除する。
            while (true)
            {
                //wait for a　画像の削除周期(deletePicTimerInterval) to come.
                Thread.Sleep(GlobalConstants.deletePicTimerInterval);




                //Task ②
                //画像を削除する前にその画像がSaveListに存在しているかどうかを確認する。
                //存在している場合、今回削除をやめて、次の周期で削除する。
                #region Make sure the DeleteList will not be cleared when "the picture to be deleted is being saved at the same time" happens
                //Make sure the picture will not be deleted before being saved to NAS 
                picHasNotBeenSaved = false;

                #endregion



                //Task ③
                //SaveList内の画像数が設定された閾値を超えた場合、
                //「NASに画像保存進捗非常に遅い」とみなす。
                //pop-upメッセージでエラーメッセージを表示し、
                //画像を削除する前にその画像がSaveListに存在しているかどうかを確認することをやめて、
                //無条件で画像を直接削除する。
                checkIfSavePicToNASTooSlow(ref SaveToNASTooSlow_DeletePicNoMatterWhat);



                //use the lock for this camera's DeleteList to avoid
                //the Deletelist from being added picture names into it  
                //by the functions in ManageCameraFolder.cs and being removed picture names from it  
                //by the functions in DeleteFiles.cs at the same time.
                lock (GlobalConstants.DeleteList_Lock[DeleteList_LockIndex])
                {
                   

                    //Delete old pictures only if DeleteList is not empty
                    deleteListCount_BeforeDeletion = currentDeleteList.Count;
                    if (deleteListCount_BeforeDeletion > 0)
                    { 
                        //DeleteList中の画像と各画像の情報ファイルを全部削除する
                        for (int i = 0; i < deleteListCount_BeforeDeletion; i++)
                        {


                            //DeleteList中の画像のパスを作る
                            picToBeDeleteFilePath = cameraPicFolderPath + "\\" + currentDeleteList[i];


                            //削除された画像の情報ファイルのパスを作る
                            infoFileToBeDeleteFilePath = cameraPicFolderPath + "\\" + Path.ChangeExtension(currentDeleteList[i], GlobalConstants.infoFileType);


                            //Task ②
                            //画像を削除する前にその画像がSaveListに存在しているかどうかを確認する。
                            //存在している場合、今回削除をやめて、次の周期で削除する。
                            #region Make sure the DeleteList will not be cleared when "the picture to be deleted is being saved to NAS at the same time" happens
                            //If the picture to be deleted exists, and is being saved to NAS at the same time
                            //we stop deleting pictures and wait for the next deletion period to come.
                            //(on the premise that "画像をNASに保存する機能" is not out of order)
                            if (currentSaveList.Count > 0 && SaveToNASTooSlow_DeletePicNoMatterWhat == false)
                            {
                                if (currentDeleteList[i] == currentSaveList[0])
                                {
                                    if (File.Exists(picToBeDeleteFilePath) && File.Exists(infoFileToBeDeleteFilePath))
                                    {
                                        //to indicate "the picture to be deleted is being saved to NAS at the same time" happened
                                        picHasNotBeenSaved = true;

                                        //to record the index of the picture in the DeleteList that is to be deleted and saved to NAS at the same time
                                        //so that we can delete the name of pictures from the DeleteList 
                                        //that have been deleted from the camera folder.
                                        picDeleteSaveAtSameTime_Index = i;

                                        //Stop deleting pictures(do not clear the DeleteList).
                                        //We will delete the pictures that haven't been saved to NAS 
                                        //at the next deletion period.
                                        break;
                                    }
                                }
                            }
                            #endregion

                            //DeleteList中の画像をcamera folderから削除する
                            deleteAFileWithRetries(picToBeDeleteFilePath);

                            //削除された画像の情報ファイルもcamera folderから削除する
                            deleteAFileWithRetries(infoFileToBeDeleteFilePath);





                        }


                        //Remove all the deleted pictures from DeleteList if
                        //"the picture to be deleted is being saved to NAS at the same time" does not happen,
                        //or "画像をNASに保存する機能" is out of order
                        if (picHasNotBeenSaved == false)
                        {
                            //DeleteListの内容を全部消す
                            currentDeleteList.Clear();

                        }
                        else
                        {
                            //If "the picture to be deleted is being saved to NAS at the same time" happened,
                            //delete the name of pictures from the DeleteList 
                            //that have been deleted from the camera folder.
                            currentDeleteList.RemoveRange(0, picDeleteSaveAtSameTime_Index);


                        }
                    }

                }


                
            }
        }





        //Task ③
        //SaveList内の画像数が設定された閾値を超えた場合、
        //「NASに画像保存進捗非常に遅い」とみなす。
        //pop-upメッセージでエラーメッセージを表示し、
        //画像を削除する前にその画像がSaveListに存在しているかどうかを確認することをやめて、
        //無条件で画像を直接削除する。
        private void checkIfSavePicToNASTooSlow(ref bool SaveToNASTooSlow_DeletePicNoMatterWhat)
        {
            #region Check if 「NASに画像保存進捗非常に遅い」 happened
            //If there are more than NotYetBeSavedToNASPicLimit pictures in SaveList,
            //we regard it as 「NASに画像保存進捗非常に遅い」
            //the "画像をNASに保存する機能" is almost stopped.
            if (currentSaveList.Count >= GlobalConstants.waitToBeSavedToNASPicNumLimit)
            {
                //Delete all the pictures accumulated in the DeleteList
                SaveToNASTooSlow_DeletePicNoMatterWhat = true;

                //show a pop-up message to inform the operator to reboot the system
                //because the "画像をNASに保存する機能"  crashed unexpectedly.

                //display this error message to inform the user
                string errorMessage = GlobalConstants.TCPSocketServerName + "は予兆と事後前後の画像をNASに保存する進捗が非常に遅いため、" +
                    "\nNASとの接続やNASの負荷に問題が発生した恐れがある。" +
                    "\n\n停止ボタンを押してを停止して、\n管理者に連絡してください。" + "\n\n\n管理者へのメッセージ:\nカメラフォルダー容量不足防止策として、\n画像を定期的に削除する機能がNASに保存したい画像の保存進捗を確認せず、\n無条件で削除を実施し始めた。" + "\n\n\n管理者への解決手順：\n" + "Step1 NASとの接続やNASの状態を確認してください。\nStep2 Step1 は問題がない場合、" + GlobalConstants.TCPSocketServerName + "の画像をNASに保存する機能の修正が必要となります。";
                string errorMessageBoxTitle = " " + GlobalConstants.TCPSocketServerName + " NASに画像保存進捗非常に遅い";
                string NASErrorTxtFileName = "NASに画像保存進捗非常に遅い";

                //show the error message box (if currently no error message box is shown) to inform the user and upload the error message to the アプリ_エラーメッセージ folder on NAS
                ReportErrorMsg.showMsgBoxIfNotShown_UploadErrMsgToNAS(errorMessage, errorMessageBoxTitle, NASErrorTxtFileName);

                      
                //wait a while to let the program output the 3rd retry error txt message
                //The 3rd retry error message can not be output without this wait 
                Thread.Sleep(1000);
            }


            #endregion
        }




        //Delete a specified file.
        //Retry 3 times if the deletion fails.
        public static void deleteAFileWithRetries(string targetFilePath)
        {
            //Check whether the folder exists, if not, don't do anything
            if (File.Exists(targetFilePath))
            {

                

                //Retry  when error occurs during compression.
                for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
                {

                    try
                    {
                        //Delete the file
                        File.Delete(targetFilePath);

                        //end this function if no error occurs
                        return ;
                    }

                    //削除する直前、対象画像が消えた
                    catch (DirectoryNotFoundException　e)
                    {
                        //if the file to be deleted does not exist
                        //output error message but do not retry

                        //output this error message to inform the user
                        string errorMessage = targetFilePath + " を削除する直前、"+ targetFilePath+"が突然消えてしまった。\n\n" + "エラーメッセージ：\n" + e.Message;

                        //output the error message                            
                        //to the DataManagementApp_エラーメッセージ folder in NAS
                        ReportErrorMsg.outputErrorMsg("ファイル削除", errorMessage);

                        //wait a while to let the program output the error txt message.  
                        Thread.Sleep(1000);

                        //do not retry
                        return;
                    }
                    
                    //For the rest of the exceptions,
                    //for example, "the specified file is in use",
                    //we retry and output error message when the retry times limit is reached.
                    catch (Exception e)
                    {
                        
                        
                        string errorMessage = "";
                        // If it's still within retry times limit
                        if (retryTimes < GlobalConstants.retryTimesLimit)
                        {

                           
                            //wait a while before starting next retry
                            Thread.Sleep(GlobalConstants.retryTimeInterval);


                        }

                        //If it has reached the retry limit
                        else
                        {

                            //Check if number of picture that failed to be deleted has reached the limit 
                            //if so, display error message box to ask the user to reboot the system.
                            checkFailToDeletePicNumWarningLimit();



                            //output this error message to inform the user
                            errorMessage = targetFilePath + " を削除する途中でエラーが発生しました。\n今回は " + retryTimes + "回目のRetryです。\nRetry回数の上限に達しましたので、Retryしません。\n\nこのファイルを削除するのを諦めて、次のファイルを削除する。\n\n" + "エラーメッセージ：\n" + e.Message;


                            //output the error message                         
                            //to the DataManagementApp_エラーメッセージ folder in NAS
                            ReportErrorMsg.outputErrorMsg("ファイル削除", errorMessage);

                            //wait a while to let the program output the 3rd retry error txt message
                            //The 3rd retry error message can not be output without this wait 
                            Thread.Sleep(1000);

                            //Give up deleting this file 
                            //and end this function becasue we have reached the limit of retry
                            return;

                        }

                    }


                    
                    

                }

               
            }

            

       
        }



        //Check if number of picture that has deletion failure has reached the limit 
        //If so, display error message box to ask the user to stop the system and contact 管理者 for help.
        private static void checkFailToDeletePicNumWarningLimit()
        {
            --GlobalConstants.failToDeletePicNumWarningLimit;

            if(GlobalConstants.failToDeletePicNumWarningLimit<=0)
            {
                //show a pop-up message to inform the operator to reboot the system
                //because one of the App1~6 crashed unexpectedly.

                //show the error message box to inform the user if no message box is being shown 
                ReportErrorMsg.showMsgBox_IfNotShown(GlobalConstants.TCPSocketServerName+ "は定期的に古い画像を削除する機能が削除できない画像が多すぎる。\n\n停止ボタンを押してを停止して、\n管理者に連絡してください。"+ "\n\n管理者への解決手順：\nStep1 古い画像の保存先から手動でファイルを削除できるかどうかを確認してください。\nStep2　Step1 は問題がない場合、" + GlobalConstants.TCPSocketServerName+ "の定期的に古い画像を削除する機能の修正が必要となります。", " " + GlobalConstants.TCPSocketServerName+ "_削除できない画像が多すぎるエラー");

            }
        }

        
       
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DataManagementApp
{
    class CopyPicsToNAS
    {


        //定期的にSaveList内の画像をNASに保存する。
        public void copyCamerasPicToNAS()
        {


            while (true)
            {
                //wait for a while(copyPicToNASTimerInterval)
                Thread.Sleep(GlobalConstants.copyPicToNASTimerInterval);



                #region 前後画像の保存

                //ずっと順番にカメラ１～カメラ6のSaveListから
                //設定枚数分の画像をNASに移動して保存する
                for (int cameraNo = 1; cameraNo <= GlobalConstants.numOfCameraSaveToNAS; cameraNo++)
                {

                    //If the camera folder path is not set in the 
                    //設定パラメーターTxtファイル,
                    //it means the user doesn't want to save pictures of that camera;
                    //therefore,we do nothing with the pictures taken by that camera.
                    if (GlobalConstants.SaveBeforeAfterAlarmPic[cameraNo] == false)
                    {
                        continue;
                    }

                    switch (cameraNo)
                    {
                        case 1:
                            //カメラ1のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera1SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera1SaveList, GlobalConstants.camera1PicFolderPath, GlobalConstants.camera1NASAlarmPicStart_FolderPath, ref GlobalConstants.camera1NASMonthRecorder, ref GlobalConstants.camera1NASDayRecorder, ref GlobalConstants.camera1NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera1SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 2:
                            //カメラ2のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera2SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera2SaveList, GlobalConstants.camera2PicFolderPath, GlobalConstants.camera2NASAlarmPicStart_FolderPath, ref GlobalConstants.camera2NASMonthRecorder, ref GlobalConstants.camera2NASDayRecorder, ref GlobalConstants.camera2NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera2SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 3:
                            //カメラ3のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera3SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera3SaveList, GlobalConstants.camera3PicFolderPath, GlobalConstants.camera3NASAlarmPicStart_FolderPath, ref GlobalConstants.camera3NASMonthRecorder, ref GlobalConstants.camera3NASDayRecorder, ref GlobalConstants.camera3NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera3SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 4:
                            //カメラ4のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera4SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera4SaveList, GlobalConstants.camera4PicFolderPath, GlobalConstants.camera4NASAlarmPicStart_FolderPath, ref GlobalConstants.camera4NASMonthRecorder, ref GlobalConstants.camera4NASDayRecorder, ref GlobalConstants.camera4NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera4SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 5:
                            //カメラ5のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera5SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera5SaveList, GlobalConstants.camera5PicFolderPath, GlobalConstants.camera5NASAlarmPicStart_FolderPath, ref GlobalConstants.camera5NASMonthRecorder, ref GlobalConstants.camera5NASDayRecorder, ref GlobalConstants.camera5NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera5SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 6:
                            //カメラ6のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            GlobalConstants.camera6SaveListNotExistPicCounter = copyPicInListToNAS(GlobalConstants.SaveList_Lock[cameraNo - 1], GlobalConstants.camera6SaveList, GlobalConstants.camera6PicFolderPath, GlobalConstants.camera6NASAlarmPicStart_FolderPath, ref GlobalConstants.camera6NASMonthRecorder, ref GlobalConstants.camera6NASDayRecorder, ref GlobalConstants.camera6NASHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (GlobalConstants.camera6SaveListNotExistPicCounter == -1)
                            {
                                return;
                            }
                            break;
                    }
                }
                #endregion



                #region オフライン検証用画像の保存


                //ずっと順番にカメラ１～カメラ6のValidationListから
                //設定枚数分の画像をNASに移動して保存する
                for (int cameraNo = 1; cameraNo <= GlobalConstants.numOfCameraUpperLimit; cameraNo++)
                {


                    switch (cameraNo)
                    {
                        case 1:
                            //カメラ1のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            int notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera1ValidationList, GlobalConstants.camera1PicFolderPath, GlobalConstants.camera1NASValidationPicStart_FolderPath, ref GlobalConstants.camera1NASValidationMonthRecorder, ref GlobalConstants.camera1NASValidationDayRecorder, ref GlobalConstants.camera1NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 2:
                            //カメラ2のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera2ValidationList, GlobalConstants.camera2PicFolderPath, GlobalConstants.camera2NASValidationPicStart_FolderPath, ref GlobalConstants.camera2NASValidationMonthRecorder, ref GlobalConstants.camera2NASValidationDayRecorder, ref GlobalConstants.camera2NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 3:
                            //カメラ3のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera3ValidationList, GlobalConstants.camera3PicFolderPath, GlobalConstants.camera3NASValidationPicStart_FolderPath, ref GlobalConstants.camera3NASValidationMonthRecorder, ref GlobalConstants.camera3NASValidationDayRecorder, ref GlobalConstants.camera3NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 4:
                            //カメラ4のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera4ValidationList, GlobalConstants.camera4PicFolderPath, GlobalConstants.camera4NASValidationPicStart_FolderPath, ref GlobalConstants.camera4NASValidationMonthRecorder, ref GlobalConstants.camera4NASValidationDayRecorder, ref GlobalConstants.camera4NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 5:
                            //カメラ5のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera5ValidationList, GlobalConstants.camera5PicFolderPath, GlobalConstants.camera5NASValidationPicStart_FolderPath, ref GlobalConstants.camera5NASValidationMonthRecorder, ref GlobalConstants.camera5NASValidationDayRecorder, ref GlobalConstants.camera5NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;

                        case 6:
                            //カメラ6のSaveListから一回設定枚数分の画像をNASに移動して保存する
                            notExistPicCounter = copyPicInListToNAS(GlobalConstants.ValidationList_Lock[cameraNo - 1], GlobalConstants.camera6ValidationList, GlobalConstants.camera6PicFolderPath, GlobalConstants.camera6NASValidationPicStart_FolderPath, ref GlobalConstants.camera6NASValidationMonthRecorder, ref GlobalConstants.camera6NASValidationDayRecorder, ref GlobalConstants.camera6NASValidationHourRecorder);
                            //If we fail to create a new folder on the NAS 
                            //for saving images after 3 retries,
                            //we output a popup error message and stop the 前後の画像をNASに移動して保存する機能
                            if (notExistPicCounter == -1)
                            {
                                return;
                            }
                            break;
                    }
                }
                #endregion

            }



        }




        //Task ①Listを確認し、Listに画像があれば、毎回設定枚数分の画像をNASに保存する。
        //Task ②保存する画像の生成Hour,Day,Monthに変換があった場合、NASに新しいフォルダを生成して画像をそのフォルダに保存する。
        private int copyPicInListToNAS(object lockObj, List<string> currentList, string cameraPicFolder,string cameraNASEachStart_FolderPath, ref int currentCameraNASMonthRecorder, ref int currentCameraNASDayRecorder,ref int currentCameraNASHourRecorder)
        {


                //For each time we save pictures to NAS,
                //we record under normal conditions how many pictures in the SaveList 
                //that don't exist in the camera folder.
                //e.g.,if we save 100 pictures to NAS every time, 
                //and this time camera1's notExistPicCounter is 4.
                //It means the 97th～100th pictures haven't been created, 
                //and therefore don't exist in the camera folder this time.
                int notExistPicCounter = 0;

                //if nothing is in the SaveList, just leave the function
                if (currentList.Count > 0)
                {
                    /*
                    //print out the SaveList for debug

                        int pos = cameraPicFolder.LastIndexOf("\\") + 1;
                        Console.Write(cameraPicFolder.Substring(pos, cameraPicFolder.Length - pos));
                        Console.WriteLine(" SaveList before upload to NAS:");
                        for (int i = 0; i < currentList.Count; i++)
                        {
                            Console.Write(currentList[i]);
                        }
                        Console.WriteLine();
                        Console.WriteLine();
                    //print out the SaveList for debug
                    */


                    

                    //move the current first picture name in the SaveList to NAS
                    string picToBeCopied ="";
                    string picPathInNASAfterCopy = "";

                    //store the creation time of the corresponding 情報ファイル
                    //(The creation time of 情報ファイル is the same as its content )
                    //to be the picture name when the picture is uploaded to NAS
                    DateTime picCreationTime=DateTime.Now;
                    string picNameInNASAfterCopy = "";
                    


                    for (int i = 0; i < GlobalConstants.copyPicToNASNumber; i++)
                    {
                        if (currentList.Count > 0 && notExistPicCounter < currentList.Count)
                        {
                            //Retry  when error occurs during compression.
                            for (int retryTimes = 1; retryTimes <= GlobalConstants.retryTimesLimit; retryTimes++)
                            {
                                    try
                                    {

                                        
                                            //move the current first picture name in the SaveList to NAS
                                            picToBeCopied = cameraPicFolder + "\\" + currentList[notExistPicCounter];
                                            
                                            
                                            //if the picture to be copied to NAS has been created,
                                            //we copy the picture to NAS.
                                            if (File.Exists(picToBeCopied))
                                            {

                                    　　　　　　//画像の生成時間を取得し、NASに保存する時、この生成時間でリネームする。
                                                try
                                                {
                                                    //Get the creation time of the picture to be copied to NAS.
                                                    picCreationTime = File.GetCreationTime(picToBeCopied);
                                                    
                                                    //We will use this to rename the picture before we upload it to NAS.
                                                    picNameInNASAfterCopy = picCreationTime.ToString("yyyyMMdd_HHmmss.fff");
                                               

                                                }
                                                catch (Exception e)
                                                {
                                                    //If the FormatException happens, we use one second after the previous 情報ファイルの生成時間  as the 情報ファイルの生成時間 of this 切り替わり image.        
                                                    picCreationTime = picCreationTime.AddSeconds(1);

                                                    //If the FormatException happens, we use current timestamp as the 情報ファイルの生成時間 of this 切り替わり image.
                                                    picNameInNASAfterCopy = "画像の生成時間の取得異常発生_例外処理でファイル名を作った" + picCreationTime.ToString("yyyyMMdd_HHmmss.fff");

                                                    //output the error message
                                                    //to the アプリ_エラーメッセージ folder in NAS
                                                    string errorMessage = "画像の生成時間を取得する時、異常が発生した。\n\nエラーメッセージ:\n" + e.Message;

                                                    ReportErrorMsg.outputErrorMsg("画像の生成時間を取得する時エラー", errorMessage);

                                                    Thread.Sleep(1000);
                                                }


                                                #region 保存する画像の生成時間に変換があれば、NAS上に新しいフォルダを生成して画像を保存する。
                                                string finalDestinationFolder = CreateFolders.createNewFolder_IfPicCreationTimeHourChange( cameraNASEachStart_FolderPath, picCreationTime, ref currentCameraNASMonthRecorder, ref currentCameraNASDayRecorder, ref currentCameraNASHourRecorder);
                                                if (finalDestinationFolder == "新しいフォルダー作成失敗")
                                                {
                                                    //前後の画像保存機能を停止させ、エラーメッセージを表示
                                                    //システムを停止してもらい、NASにフォルダーを生成できるようになってから、
                                                    //システムを再開してもらう。
                                                    return -1;
                                                }
                                                #endregion

                                                //rename the picture with the creation time of its 情報ファイル
                                                picPathInNASAfterCopy = finalDestinationFolder + "\\" + picNameInNASAfterCopy + "." + GlobalConstants.imageType;

                                                //Copy the picture to NAS
                                                File.Copy(picToBeCopied, picPathInNASAfterCopy);

                                                //Remove the saved picture name from the SaveList

                                                //Use the lock for this camera's SaveList to avoid
                                                //the Savelist from being added picture names into it  
                                                //by the functions in the ManageSaveList.cs and being removed picture names from it  
                                                //by thefunctions in the CopyPicsToNAS.cs at the same time.
                                                lock (lockObj)
                                                {
                                                    //Under normal conditions,there is no way that the pictures that haven't been created 
                                                    //appear prior to a picture that exists.
                                                    //If this happened, that means for some reason these pictures are not created,
                                                    //so we need to remove them from the List.
                                                    if (notExistPicCounter > 0)
                                                    {
                                                        currentList.RemoveRange(0, notExistPicCounter);
                                                        //Reset the notExistPicCounter
                                                        notExistPicCounter = 0;


                                                    }
                                                    //Remove the currently saved picture name from the SaveList
                                                    currentList.RemoveAt(notExistPicCounter);
                                                }

                                            }

                                            //if the picture has not been created,
                                            //we skip it and increase the counter of the nonexistent picture
                                            //to point to the next element in SaveList.
                                            else
                                            {
                                                if (notExistPicCounter < currentList.Count)
                                                {
                                                    //increase the counter of the nonexistent picture
                                                    notExistPicCounter = notExistPicCounter + 1;
                                                }
                                            }
                                        

                                        //end the retry if no error occurs
                                        break;
                                    }
                                    
                                    //If for some abnormal reason, the picture disappears when we are saving it,
                                    //we give up saving the picture to NAS without retry,and output an error message.
                                    catch(FileNotFoundException e)
                                    {

                                        
                                        string errorMessage = "";
                                        //display this error message to inform the user
                                        errorMessage = picToBeCopied + " をNASに保存する途中でエラーが発生しました。\nこの画像が存在していないので、\nこの画像をNASに保存するのを諦めて、次の画像を保存する。\n\n" + "エラーメッセージ：\n" + e.Message;

                                        //output the error message  
                                        //to the DataManagementApp_エラーメッセージ folder in NAS
                                        ReportErrorMsg.outputErrorMsg("画像をNASに保存", errorMessage);


                                        //Remove the picture name that doesn't exist in the camera folder from the SaveList
                                        
                                        //Use the lock for this camera's SaveList to avoid
                                        //the Savelist from being added picture names into it  
                                        //by the functions in the ManageSaveList.cs and being removed picture names from it  
                                        //by thefunctions in the CopyPicsToNAS.cs at the same time.
                                        lock (lockObj)
                                        {
                                            currentList.RemoveAt(notExistPicCounter);
                                        }


                                        //wait a while to let the program output the error txt message
                                        //error message can not be output without this wait 
                                        Thread.Sleep(1000);

                                        //do not retry for this exception
                                        break;
                                    }


                                    //Retry for 3 times and output error message  
                                    //for the rest of the exceptions which happen while copying pictures to NAS
                                    catch (Exception e)
                                    {
                                        string errorMessage = "";

                                        // If it's still within retry times limit
                                        if (retryTimes < GlobalConstants.retryTimesLimit)
                                        {
                                            
                                            
                                            //display this error inform the user
                                            errorMessage = picToBeCopied + " をNASに保存する途中でエラーが発生しました。\n今もう一度NASに保存してみます。\n今回は " + retryTimes + "回目のRetryです。\n\n" + "エラーメッセージ：\n" + e.Message;

                                            //output the error message
                                            //to the DataManagementApp_エラーメッセージ folder in NAS
                                            ReportErrorMsg.outputErrorMsg("画像をNASに保存", errorMessage);

                                            //wait a while before starting next retry
                                            Thread.Sleep(GlobalConstants.retryTimeInterval);


                                            //Handling the picture is not completely saved to NAS due to the 
                                            //予期しないネットワーク エラーが発生しました。
                                            //We delete the picture in the NAS if it exists 
                                            //because it is not completely saved to NAS
                                            //due to the network error.
                                            if (e.Message == "予期しないネットワーク エラーが発生しました。\r\n")
                                            {

                                                DeleteCameraFolderPic.deleteAFileWithRetries(picPathInNASAfterCopy);
                                                
                                            }


                                        }

                                        //If it has reached the retry limit
                                        else
                                        {

                                            
                                           
                                            //display this error message to inform the user
                                            errorMessage = picToBeCopied + " をNASに保存する途中でエラーが発生しました。\n今回は"+ GlobalConstants.retryTimesLimit + "回目のRetryです。\nRetry回数の上限に達しましたので、\nこの画像をNASに保存するのを諦めて、次の画像を保存する。\n\n" + "エラーメッセージ：\n" + e.Message;

                                            //show a pop-up message to inform the operator that if this error
                                            //occurs so many times, contact 管理者 for help.
                                            ReportErrorMsg.showMsgBox_IfNotShown("予兆と事後前後の画像一枚を" + GlobalConstants.retryTimesLimit + "回RetryしてもNASに保存できない。\nその画像を保存するのを諦めて、次の画像を保存する。\n\nこのエラー何回も発生した場合、\n停止ボタンを押してを停止して、\n管理者に連絡してください。" + "\n\n管理者への解決手順：\nStep1 NASへの接続とNASの状態を確認してください。\nStep2 システムを再起動して、このエラーが長い時間で一回だけ発生した場合、単純に一時的なNASへの接続不具合だからです。\n多発している場合、予兆と事後前後の画像をNASに保存する機能の修正が必要となります。" + "\n\nこのエラーの原因：\n" + e.Message, " " + GlobalConstants.TCPSocketServerName +" 画像をNASに保存できないエラー");


                                            //output the error message 
                                            //to the DataManagementApp_エラーメッセージ folder in NAS
                                            ReportErrorMsg.outputErrorMsg("画像をNASに保存", errorMessage);



                                            //Remove the picture name that causes error from the SaveList
                                            //We give up saving the problem picture to NAS and proceed to save the next picture in SaveList
                                            
                                            //Use the lock for this camera's SaveList to avoid
                                            //the Savelist from being added picture names into it  
                                            //by the functions in the ManageSaveList.cs and being removed picture names from it  
                                            //by thefunctions in the CopyPicsToNAS.cs at the same time.
                                            lock (lockObj)
                                            {
                                                currentList.RemoveAt(notExistPicCounter);
                                            }



                                            //wait a while to let the program output the 3rd retry error txt message
                                            //The 3rd retry error message can not be output without this wait 
                                            Thread.Sleep(1000);




                                        }

                                    }
                                    
                                    

                            }

                        }
                        else //end this function because notExistPicCounter has reached the end of the currentList or the currentList is empty         
                        {
                           
                            //break the loop of saving GlobalConstants.copyPicToNASNumber pictures to NAS
                            break;
                        }
                            
                            
                    }

                    #region Remove all the pictures we want to save this time from SaveList if none of them exist
                            //if the currentList is not empty and "picPathInNASAfterCopy" varialbe is not used for at least one time,
                            //it means all the pictures that we want to save this time  don't exist in the camera folder.
                            //In this case, we remove those pictures from the currentList to 
                            //prevent NASに保存する際に画像名が重複している問題 in the future.
                            
                            if (currentList.Count > 0　&& picPathInNASAfterCopy=="")
                            {
                                //Use the lock for this camera's SaveList to avoid
                                //the Savelist from being added picture names into it  
                                //by the functions in the ManageSaveList.cs and being removed picture names from it  
                                //by thefunctions in the CopyPicsToNAS.cs at the same time.
                                lock (lockObj)
                                {
                                    currentList.RemoveRange(0, notExistPicCounter);
                                }
                            }
                     #endregion

                }

                return notExistPicCounter;
            
        }

            
        
    }
}

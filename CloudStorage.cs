using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CG.Web.MegaApiClient;
using System.Linq;
using System;
using System.IO;

public class CloudStorage : MonoBehaviour
{
    private MegaApiClient client;
    private string megaUsername = "sahilushaikh6@gmail.com";
    private string megaPassword = "Sahmain@9876";


    #region UnityAPI
    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        //FolderDownloadContent("https://mega.nz/folder/ynZmnLbJ#0QmW68kzBEALh3Cy-ITEFw");
        // ListFiles();
        //Testdf();
        //  GetFolder("Companyname","Databasename","th.jpg");
        //  DeleteFile("Companyname","Databasename","th.jpg");
        // CreateFolderOnServer();
        // Test();
     //   GetSubFolders("Companyname");
        GetAllFilesFromFolder("Test");
     //   UploadFileOnFolders("Test", "C:/Users/SAHIL/Desktop/Test/Test1/1.png");
    }

    #endregion

    #region CreateFolder

    void CreateFolder(string parentFoldername , string childFoldername)
    {

        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        try
        {
            IEnumerable<INode> nodes = client.GetNodes();
            INode parentfolder = GetPaticularFolder(parentFoldername);
            INode myFolder = client.CreateFolder(childFoldername, parentfolder);

        }
        catch (Exception e)
        {

            throw;
        }

        client.Logout();
    }

    void ScreateDefaultFolder( string gamil)
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        try
        {
            IEnumerable<INode> nodes = client.GetNodes();

            INode root = nodes.Single(x => x.Type == NodeType.Directory);
            INode myFolder = client.CreateFolder(gamil, root);



        }
        catch (Exception e)
        {

            throw;
        }



        client.Logout();
    }

    #endregion

    #region DeleteFileOrFolder

   //async void DeleteFile(string Companyname, string Databasename, string Filename)
   // {
   //     try
   //     {
   //         Debug.Log(Application.dataPath);
   //         client = new MegaApiClient();
   //         client.Login(megaUsername, megaPassword);

         
   //         INode myFile = ServerFileReference(Companyname, Databasename, Filename);
           


   //         IProgress<double> progressHandler = new Progress<double>(x => Debug.Log("{0}%" + x));

   //         await client.DeleteAsync(myFile,true);
   //     }
   //     catch (Exception e)
   //     {
   //         Debug.Log(e.Message);



   //     }

   //}

    async void DeleteNode(INode node)
    {
        try
        {
            client = new MegaApiClient();
            client.Login(megaUsername, megaPassword);

            IProgress<double> progressHandler = new Progress<double>(x => Debug.Log("{0}%" + x));

            await client.DeleteAsync(node, true);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);



        }
    } // delete Perticular file or node



    #endregion

    #region DownloadFile
    public async void DownloadFile(INode myFile, string Filename)
    {
        try
        {
            Debug.Log(Application.dataPath);
            client = new MegaApiClient();
            client.Login(megaUsername, megaPassword);

            IProgress<double> progressHandler = new Progress<double>(x => Debug.Log("{0}%" + x));

            await client.DownloadFileAsync(myFile, Application.dataPath + "/" + Filename, progressHandler);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);



        }
    }

    async void DownloadfromLink(string link)
    {
        client = new MegaApiClient();
        client.LoginAnonymous();

        Uri fileLink = new Uri(link);
        INode node = client.GetNodeFromLink(fileLink);

        Console.WriteLine($"Downloading {node.Name}");
        IProgress<double> progressHandler = new Progress<double>(x => Debug.Log("{0}%" + x));
        await client.DownloadFileAsync(fileLink, node.Name);

        client.Logout();
    }

    #endregion

    #region GetFilesOrFolder
    void GetAllFilesFromFolder(string Foldername) // Get All the Filse From The folder
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        INode Folder = GetPaticularFolder(Foldername);
        IEnumerable<INode> files = client.GetNodes(Folder);
        List<INode> allFiles = files.Where(n => n.Type == NodeType.File).ToList();

        foreach (var subDir in allFiles.Where(node => node.Type == NodeType.File))
        {
            Debug.Log(subDir.Name + client.GetDownloadLink(subDir));
        }
        client.Logout();
    }
    void GetAllSubFolders(string parentFolder) // Get Sub Folders from root folders
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        IEnumerable<INode> company = client.GetNodes();
        INode companyRoot = company.SingleOrDefault(node => node.Type == NodeType.Root);

        if (companyRoot != null)
        {
            // Find the "Companyname" directory
            INode companyNameDir = company.SingleOrDefault(node => node.Type == NodeType.Directory && node.Name == parentFolder);

            if (companyNameDir != null)
            {
                IEnumerable<INode> subDirectories = client.GetNodes(companyNameDir);

                foreach (var subDir in subDirectories.Where(node => node.Type == NodeType.Directory))
                {
                    Debug.Log(subDir.Name);
                }
            }
        }

        client.Logout();
    }


    #endregion

    #region Helper

    INode GetPaticularFolder(string Name) // Get perticular folder
    {
        IEnumerable<INode> Company = client.GetNodes();
        List<INode> CompanyRef = Company.Where(n => n.Type == NodeType.Directory).ToList();
       
       
        return CompanyRef.FirstOrDefault(f => f.Name == Name);
     
    }
  

   
    void UploadFileOnFolders(string Foldername , string FileLoaction) // Upload File in folder
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        INode Folder = GetPaticularFolder(Foldername);
        INode myFile = client.UploadFile(FileLoaction, Folder);

        Uri downloadLink = client.GetDownloadLink(myFile);
        Debug.Log(downloadLink.ToString());
        client.Logout();
    }

    //GetAllFiles Indside Folders

    #endregion

    #region Test
    void Test()
    {
       // string megaUsername = "YourMegaUsername";
       // string megaPassword = "YourMegaPassword";
        string localFolderPath = @"C:\Users\SAHIL\Desktop\Test"; // Replace with your local folder path

        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);

        IEnumerable<INode> nodes = client.GetNodes();
        INode root = nodes.Single(x => x.Type == NodeType.Root);
        UploadFolder(client, localFolderPath, root);

        client.Logout();
    }

     void UploadFolder(MegaApiClient client, string localFolderPath, INode parent)
    {
        DirectoryInfo localDir = new DirectoryInfo(localFolderPath);
        INode megaFolder = client.CreateFolder(localDir.Name, parent);

        foreach (var file in localDir.GetFiles())
        {
            client.UploadFile(file.FullName, megaFolder);
        }

        foreach (var subDir in localDir.GetDirectories())
        {
            UploadFolder(client, subDir.FullName, megaFolder);
        }
    }
    #endregion

}

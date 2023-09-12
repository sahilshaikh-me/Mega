using System.Collections.Generic;
using UnityEngine;
using CG.Web.MegaApiClient;
using System.Linq;
using System;
using System.IO;

public class CloudStorage : MonoBehaviour
{
    #region privateVariables
    private MegaApiClient client;
    private string megaUsername = "*********@gmail.com";
    private string megaPassword = "******@9876";
    #endregion

    #region UnityAPI
    private void Start()
    {
        //   GetAllSubFolders("Companyname");
        //  CreateFolder("Companyname","SahilFolderTest");
        //foreach (var item in GetAllFilesFromFolder("Companyname"))
        //{
        //     Debug.Log(item.Key.ToString()+" " +item.Value.ToString());

        //}
        //   UploadFileOnFolder("Test", "C:/Users/SAHIL/Desktop/Test/Sahil.jpg");
      //  Renamenode("Databasename", "DBname");

    }

    #endregion

    #region CreateFolder
    public bool isFolderPresent = true;
    async void CreateFolder(string parentFoldername , string childFoldername)
    {

        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        
        try
        {
                IEnumerable<INode> nodes = client.GetNodes();
                INode parentfolder = GetPaticularFolder(parentFoldername);
                string persistentPath = Path.Combine(Application.persistentDataPath, parentFoldername, childFoldername);
         
            foreach (var item in GetAllSubFolders(parentFoldername))
            {
                if (item != childFoldername)
                {
                    isFolderPresent = false;
                    Debug.Log("Folder created in Unity: ");
                }
                else
                {
                    isFolderPresent = true;
                    Debug.Log("Folder already exists in Unity: ");
                    return;

                }
            }
            if (!isFolderPresent)
            {
                INode folder = await client.CreateFolderAsync(childFoldername, parentfolder);
                Directory.CreateDirectory(persistentPath);
                Debug.Log("folder Created");

            }

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

    #region Rename
    public  async void Renamenode(string filderorfile, string rename)
    {
        INode test = GetPaticularFolder(filderorfile);
        await  client.RenameAsync(test, rename);
        Debug.Log("Name Changed Successfully");
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

    async void DeleteNode(INode node,string name)
    {
        try
        {
            client = new MegaApiClient();
            client.Login(megaUsername, megaPassword);
            if(node.Name == name)
            {
                IProgress<double> progressHandler = new Progress<double>(x => Debug.Log("{0}%" + x));

                await client.DeleteAsync(node, true);
            }
          
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
        await client.DownloadFileAsync(fileLink, node.Name, progressHandler);

        client.Logout();
    }

    #endregion

    #region GetFilesOrFolder
    Dictionary<string,string> GetAllFilesFromFolder(string Foldername) // Get All the Filse From The folder
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        INode Folder = GetPaticularFolder(Foldername);
        IEnumerable<INode> files = client.GetNodes(Folder);
        List<INode> allFiles = files.Where(n => n.Type == NodeType.File).ToList();
        Dictionary<string, string> test = new Dictionary<string, string>();
        foreach (var subDir in allFiles.Where(node => node.Type == NodeType.File))
        {
           /* Debug.Log(subDir.Name + client.GetDownloadLink(subDir));*/
            test.Add(subDir.Name, client.GetDownloadLink(subDir).ToString());

        }
        return test;
    }
    public List<string> GetAllSubFolders(string parentFolder) // Get Sub Folders from root folders
    {
        
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        IEnumerable<INode> company = client.GetNodes();
        INode companyRoot = company.SingleOrDefault(node => node.Type == NodeType.Root);
        List<string> subFolderNames = new List<string>();

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
                    subFolderNames.Add(subDir.Name);
                }
            }
        }

        return subFolderNames;
    }


    #endregion

    #region Helper

    INode GetPaticularFolder(string Name) // Get perticular folder
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);
        IEnumerable<INode> Company = client.GetNodes();
        List<INode> CompanyRef = Company.Where(n => n.Type == NodeType.Directory).ToList();
       
       
        return CompanyRef.FirstOrDefault(f => f.Name == Name);
     
    }

    #endregion

    #region UploadFile
    void UploadFileOnFolder(string Foldername, string FilePath) // Upload File in folder
    {
        client = new MegaApiClient();
        client.Login(megaUsername, megaPassword);

        INode Folder = GetPaticularFolder(Foldername);

        foreach (var item in GetAllFilesFromFolder(Foldername))
        {
                 Debug.Log(item.Key.ToString() + " " + Path.GetFileName(FilePath));
           
            
            if (item.Key.ToString() == Path.GetFileName(FilePath))
            {
                Debug.Log("File Already Present Delete and Upload again");
                return;
            }
            else
            {
                 INode myFile = client.UploadFile(FilePath, Folder);
                 Uri downloadLink = client.GetDownloadLink(myFile);
                Debug.Log(downloadLink.ToString());

            }

        }

        client.Logout();
    }
    #endregion


    #region Test
    void Test()// Uploading whole folder and subfolder with files
    {
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

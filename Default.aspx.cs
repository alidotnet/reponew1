using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string rootDirectory = Server.MapPath("~");
            var foundFiles = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);
            foreach (var file in foundFiles)
            {
                if (File.Exists(file))
                {
                    string content = File.ReadAllText(file, Encoding.UTF8);
                    if (content.Trim() == "")
                    {
                        content = "Webpaw created this file";
                    }
                    UploadFileToGithubAsync("alidotnet", "reponew1", "master", Path.GetFileName(file), content, "test message");
                }
            }

        }


        //public void UploadProjectToGithubRepo(string projectCode, string owner, string repo, string branch, string target, string message)
        //{
        //    ProjectClass prj = new ProjectClass();
        //    List<ProjectFilesFoldersInfo> list = prj.GetProjectFilesFolders(projectCode, "name");
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i].type == "file")
        //        {
        //            string targetFile = "";
        //            string[] a = list[i].path.Replace("///", "/").Replace("//", "/").Split('/');
        //            for (int q = 0; q < a.Length; q++)
        //            {
        //                if (q > 1)
        //                {
        //                    if (targetFile != "")
        //                    {
        //                        targetFile += "/";
        //                    }
        //                    targetFile += a[q];
        //                }
        //            }
        //            if (File.Exists(Server.MapPath("~") + "/" + list[i].path))
        //            {
        //                string content = File.ReadAllText(Server.MapPath("~") + "/" + list[i].path, Encoding.UTF8);
        //                if (content.Trim() == "")
        //                {
        //                    content = "Webpaw created this file";
        //                }
        //                UploadFileToGithubAsync(owner, repo, branch, target + "/" + targetFile, content, message);
        //            }
        //        }
        //    }
        //}

        private void UploadFileToGithub(string owner, string repo, string branch, string targetFile, string path, string message)
        {
            //UploadFileToGithubAsync(owner, repo, branch, targetFile);
        }

        private async void UploadFileToGithubAsync(string owner, string repo, string branch, string targetFile, string content, string message)
        {
            var ghClient = new GitHubClient(new ProductHeaderValue("Octokit"));
            ghClient.Credentials = new Credentials("b5b22491988fbd899a3173c78fdec06e9c019743");

            // github variables
            //var owner = "owner";
            //var repo = "repo";
            //var branch = "branch";

            //var targetFile = "_data/test.txt";
            bool updateMode = true;
            try
            {
                // try to get the file (and with the file the last commit sha)
                var existingFile = await ghClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFile, branch);

                // update the file
                var updateChangeSet = await ghClient.Repository.Content.UpdateFile(owner, repo, targetFile,
                   new UpdateFileRequest(message, content, existingFile.First().Sha, branch));
            }
            catch (Octokit.NotFoundException)
            {
                updateMode = false;
                // if file is not found, create it
            }
            if (!updateMode)
            {
                var createChangeSet = await ghClient.Repository.Content.CreateFile(owner, repo, targetFile, new CreateFileRequest(message, content, branch));
            }
        }
    }
}
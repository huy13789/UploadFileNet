using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.AccessControl;
using UploadFile.Models;

namespace UploadFile.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "Google Drive API .NET Quickstart";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            UploadFileToGoogleDrive();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Obsolete]
        private void UploadFileToGoogleDrive()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Đường dẫn tới tệp credentials.json
            string credentialsPath = Path.Combine(appDirectory, "Helper", "credentials.json");

            // Đường dẫn tới thư mục "helper"
            string _helperDirectory = Path.Combine(appDirectory, "Helper", "token.json");

            GoogleCredential credential;
            using (var stream =
                new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                //string credPath = "token.json";
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            // Tạo dịch vụ Drive.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Diractory upload file
            string filePath = "C:/Users/huy13/Downloads/LAMovies-uploadFile/LAMovies-uploadFile/LAMovies_BE/LAMovies_BE/bin/1b.mp4";

            // ID của thư mục trên Google Drive bạn muốn upload file vào.
            string folderId = "1X7dc7NCaZYgwrmpDqK6_8YEqBeWDyPXY"; 

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = "video.mp4",
                Parents = new[] { folderId }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "video/mp4");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;
            Console.WriteLine("File ID: " + file.Id);
        }

    }
}
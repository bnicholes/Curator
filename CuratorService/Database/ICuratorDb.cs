using CuratorService.Data;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
#pragma warning disable 1591

namespace CuratorService.Database
{
    internal interface ICuratorDb
    {
        ISetting GetSetting(string name);
        void SetSetting(ISetting value);
        void RemoveSetting(string name);
        IEnumerable<CuratedImage> GetCuratedImages();
        CuratedImage GetCuratedImage(string key);
        void SaveCuratedImage(CuratedImage curatedImage);
        void DeleteCuratedImage(string key);
        void DeleteCuratedImages();

        string WebSslCertificateBase64Data { get; set; }
        string WebSslCertificatePassphrase { get; set; }
        X509Certificate2 WebSslCertificate { get; set; }

        void DropDatabase();
    }
}

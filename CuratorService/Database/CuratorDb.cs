using curator.Data;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace curator.Database
{
    public class CuratorDb : ICuratorDb, IDisposable
    {
        private bool _disposed;

        private const string DbFileName = "curator.db";
        private const string ImageCollectionTableName = "imagecollection";
        private const string SettingCollectionTableName = "settingcollection";

        private LiteDatabase _curatorDb;
        private ILiteCollection<CuratedImage> _imageCollection;
        private ILiteCollection<Setting> _settingCollection;

        private const string WebSslCertificateDataKey = "WebSslCertData";
        private const string WebSslCertificatePassphraseKey = "WebSslCertPassphrase";

        public CuratorDb()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!Directory.Exists(WellKnownData.ProgramDataPath))
                Directory.CreateDirectory(WellKnownData.ProgramDataPath);
            var dbPath = Path.Combine(WellKnownData.ProgramDataPath, DbFileName);
            Serilog.Log.Logger.Information($"Loading configuration database at {dbPath}.");

            var connectionString = $"Filename={dbPath}";
            _curatorDb = new LiteDatabase(connectionString);
            //_disposed = false;
            _imageCollection = _curatorDb.GetCollection<CuratedImage>(ImageCollectionTableName);
            _settingCollection = _curatorDb.GetCollection<Setting>(SettingCollectionTableName);
        }

        private string GetSimpleSetting(string name)
        {
            var obj = GetSetting(name);
            return obj?.Value;
        }

        private void SetSimpleSetting(string name, string value)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            var obj = new Setting()
            {
                Name = name,
                Value = value ?? ""
            };
            SetSetting(obj);
        }

        public ISetting GetSetting(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            return _settingCollection.FindById(name);
        }

        public void SetSetting(ISetting value)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            _settingCollection.Upsert((Setting)value);
        }

        public void RemoveSetting(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            _settingCollection.Delete(name);
        }

        public IEnumerable<CuratedImage> GetCuratedImages()
        {
            return _imageCollection.FindAll();
        }

        public void GetCuratedImage(string key)
        {
            _imageCollection.FindById(key);
        }

        public void SaveCuratedImages(IEnumerable<CuratedImage> curatedImages)
        {
            foreach (var curatedImage in curatedImages)
            {
                _imageCollection.Upsert(curatedImage);
            }
        }

        public void DeleteCuratedImage(string key)
        {
            _imageCollection.Delete(key);
        }

        public void DeleteCuratedImages()
        {
            _imageCollection.DeleteAll();
        }

        public string WebSslCertificateBase64Data
        {
            get => GetSimpleSetting(WebSslCertificateDataKey);
            set => SetSimpleSetting(WebSslCertificateDataKey, value);
        }

        public string WebSslCertificatePassphrase
        {
            get => GetSimpleSetting(WebSslCertificatePassphraseKey);
            set => SetSimpleSetting(WebSslCertificatePassphraseKey, value);
        }

        public X509Certificate2 WebSslCertificate
        {
            get
            {
                if (!string.IsNullOrEmpty(WebSslCertificateBase64Data))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(WebSslCertificateBase64Data);
                        var cert = string.IsNullOrEmpty(WebSslCertificatePassphrase)
                            ? new X509Certificate2(bytes)
                            : new X509Certificate2(bytes, WebSslCertificatePassphrase);
                        return cert;
                    }
                    catch (Exception)
                    {
                        // TODO: log?
                        // throw appropriate error?
                    }
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    WebSslCertificateBase64Data = Convert.ToBase64String(value.Export(X509ContentType.Pfx));
                }
                else
                {
                    WebSslCertificateBase64Data = null;
                }
            }
        }

        public void DropDatabase()
        {
            Dispose();
            var dbPath = Path.Combine(WellKnownData.ProgramDataPath, DbFileName);
            File.Delete(dbPath);
            Serilog.Log.Logger.Information($"Dropped the database at {dbPath}.");

            InitializeDatabase();
        }

        public void Dispose()
        {
            _curatorDb?.Dispose();
            _disposed = true;
            _curatorDb = null;
        }

    }

}

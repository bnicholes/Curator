using CuratorService.Data;
using CuratorService.Database;
using CuratorService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace CuratorService.Logic
{
    internal class CuratorServiceLogic : ICuratorServiceLogic, IDisposable
    {
        private readonly Serilog.ILogger _logger;
        private readonly ICuratorDb _curatorDb;
        private static Random random = new Random();

        public CuratorServiceLogic(ICuratorDb curatorDb) 
        {
            _curatorDb = curatorDb;
            _logger = Serilog.Log.Logger;
        }

        private CuratorServiceException LogAndException(string msg, Exception ex = null, HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            _logger.Error(msg);
            return new CuratorServiceException(msg, ex, status);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public IEnumerable<ICuratedImage> GetCuratedImages()
        {
            var result = _curatorDb.GetCuratedImages();
            if (result == null)
            {
                return new List<ICuratedImage>();
            }

            return result;
        }

        public ICuratedImage GetCuratedImageByKey(string key)
        {
            if (key == null)
            {
                throw LogAndException("Invalid or missing key.");
            }

            var result = _curatorDb.GetCuratedImage(key);
            if (result == null)
            {
                throw LogAndException("Image not found", null, HttpStatusCode.NotFound);
            }

            return result;
        }

        public ICuratedImage SaveCuratedImage(CuratedImage curatedImage)
        {
            if (curatedImage == null)
            {
                throw LogAndException("Invalid or missing curated image data.");
            }

            if (curatedImage.Key == null)
            {
                //curatedImage.Key = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 16);
                curatedImage.Key = RandomString(16);
            }

            _curatorDb.SaveCuratedImage(curatedImage);

            return _curatorDb.GetCuratedImage(curatedImage.Key);
        }

        public ICuratedImage SaveCuratedImage(string key, CuratedImage curatedImage)
        {
            if (key == null)
            {
                throw LogAndException("Invalid or missing curated image key.");
            }

            key = Uri.UnescapeDataString(key);

            if (key != curatedImage.Key)
            {
                throw LogAndException("The key does not match the curated image key.");
            }

            if (curatedImage == null)
            {
                throw LogAndException("Invalid or missing curated image data.");
            }

            var image = _curatorDb.GetCuratedImage(key);
            if (image == null)
            {
                throw LogAndException("Image not found", null, HttpStatusCode.NotFound);
            }
            else
            {
                curatedImage.Key = key;
            }

            return SaveCuratedImage(curatedImage);
        }

        public void DeleteCuratedImage(string key)
        {
            if (key == null)
            {
                throw LogAndException("Invalid or missing curated image key.");
            }

            var image = _curatorDb.GetCuratedImage(key);
            if (image == null)
            {
                throw LogAndException("Image not found", null, HttpStatusCode.NotFound);
            }

            _curatorDb.DeleteCuratedImage(key);
        }

        public void Dispose()
        {
        }

    }
}

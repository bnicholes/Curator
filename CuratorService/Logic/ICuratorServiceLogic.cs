using CuratorService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace CuratorService.Logic
{
    public interface ICuratorServiceLogic
    {
        ICuratedImage SaveCuratedImage(CuratedImage curatedImage);
        ICuratedImage SaveCuratedImage(string key, CuratedImage curatedImage);
        IEnumerable<ICuratedImage> GetCuratedImages();
        ICuratedImage GetCuratedImageByKey(string key);
        void DeleteCuratedImage(string key);
    }
}

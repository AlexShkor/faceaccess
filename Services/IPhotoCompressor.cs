using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VueJsAspNetCoreSample.Services
{
   public interface IPhotoCompressor
    {
        string CompressPhoto(string photo);
    }
}

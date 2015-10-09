using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TNVTH.Web.Models
{
    public class AlbumViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public AlbumViewModel(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linker
{
    class Program
    {
        static void Main(string[] args)
        {
            DiaryLinker linker = new DiaryLinker();
            linker.Upload();
            Console.ReadLine();
        }
    }
}

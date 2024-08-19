using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.Data.SqlClient;
using Npgsql;


namespace FileSystemCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSystemCleaner f = new FileSystemCleaner();
            f.clear();
        }
    }
}

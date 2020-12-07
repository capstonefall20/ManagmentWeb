using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class AppSetting
{
    public static string ConnectionString { get; set; }
    public static string SecureAppUrl { get; set; }
    public static string StaticAppUrl { get; set; }
    public static string StorageApiPath { get; set; }
    public static string StoragePath { get; set; }
    public static int UserId { get; set; }
}

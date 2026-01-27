using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

public static class ExecutablePathResolver
{
    public static string GetRealExecutablePath()
    {
        var path = Environment.ProcessPath!;
        using var handle = CreateFile(
            path,
            0,
            FileShare.ReadWrite | FileShare.Delete,
            IntPtr.Zero,
            FileMode.Open,
            FileAttributes.Normal,
            IntPtr.Zero);

        if (handle.IsInvalid)
            return path; // fallback

        var buffer = new char[1024];
        var result = GetFinalPathNameByHandle(handle, buffer, buffer.Length, 0);

        if (result <= 0 || result >= buffer.Length)
            return path;

        var finalPath = new string(buffer, 0, result);

        // Windows returns paths like "\\?\C:\..."
        if (finalPath.StartsWith(@"\\?\"))
            finalPath = finalPath.Substring(4);

        return finalPath;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetFinalPathNameByHandle(
        SafeFileHandle hFile,
        char[] lpszFilePath,
        int cchFilePath,
        int dwFlags);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(
        string lpFileName,
        int dwDesiredAccess,
        FileShare dwShareMode,
        IntPtr lpSecurityAttributes,
        FileMode dwCreationDisposition,
        FileAttributes dwFlagsAndAttributes,
        IntPtr hTemplateFile);
}

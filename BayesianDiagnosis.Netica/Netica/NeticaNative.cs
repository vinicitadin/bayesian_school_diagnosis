using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace BayesianDiagnosis.Netica;

internal enum NeticaErrorSeverity
{
    NothingErr = 1,
    ReportErr = 2,
    NoticeErr = 3,
    WarningErr = 4,
    ErrorErr = 5,
    XxxErr = 6
}

[SuppressUnmanagedCodeSecurity]
internal static class NeticaNative
{
    private const string DllName = "Netica.dll";

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern IntPtr NewNeticaEnviron_ns(string license, IntPtr env, string? locn);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern int InitNetica2_bn(IntPtr env, [Out] StringBuilder mesg);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern int CloseNetica_bn(IntPtr env, [Out] StringBuilder mesg);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern IntPtr NewFileStream_ns(string filename, IntPtr env, string? access);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void DeleteStream_ns(IntPtr stream);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr ReadNet_bn(IntPtr stream, int options);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void DeleteNet_bn(IntPtr net);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void CompileNet_bn(IntPtr net);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern IntPtr GetNodeNamed_bn(string name, IntPtr net);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetNodeNumberStates_bn(IntPtr node);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr GetNodeBeliefs_bn(IntPtr node);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern IntPtr GetNodeStateName_bn(IntPtr node, int state);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    internal static extern int GetStateNamed_bn(string name, IntPtr node);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void EnterFinding_bn(IntPtr node, int state);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void RetractNetFindings_bn(IntPtr net);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr GetError_ns(IntPtr env, NeticaErrorSeverity severity, IntPtr after);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr ErrorMessage_ns(IntPtr error);

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
    internal static extern void ClearError_ns(IntPtr error);
}
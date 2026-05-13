using System.Runtime.InteropServices;
using System.Text;

namespace BayesianDiagnosis.Netica;

public sealed class NeticaEnvironment : IDisposable
{
    private const int MesgLen = 600;

    private IntPtr _env;
    private bool _initialized;
    private bool _disposed;

    public NeticaEnvironment(string license, string? location = null)
    {
        _env = NeticaNative.NewNeticaEnviron_ns(license, IntPtr.Zero, location);
        if (_env == IntPtr.Zero)
        {
            throw new InvalidOperationException("Falha ao criar o ambiente do Netica.");
        }
    }

    public void Initialize()
    {
        ThrowIfDisposed();

        if (_initialized)
        {
            return;
        }

        var mesg = new StringBuilder(MesgLen);
        var result = NeticaNative.InitNetica2_bn(_env, mesg);
        if (result < 0)
        {
            throw new InvalidOperationException($"Erro ao inicializar Netica: {mesg}");
        }

        _initialized = true;
    }

    public NeticaNetwork LoadNetwork(string path)
    {
        ThrowIfDisposed();

        if (!_initialized)
        {
            Initialize();
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Arquivo .dne não encontrado: {path}", path);
        }

        var stream = NeticaNative.NewFileStream_ns(path, _env, null);
        if (stream == IntPtr.Zero)
        {
            var error = NeticaNative.GetError_ns(_env, NeticaErrorSeverity.ErrorErr, IntPtr.Zero);
            var messagePtr = error == IntPtr.Zero ? IntPtr.Zero : NeticaNative.ErrorMessage_ns(error);
            var message = messagePtr == IntPtr.Zero ? "Erro nativo não informado." : Marshal.PtrToStringAnsi(messagePtr);
            if (error != IntPtr.Zero)
            {
                NeticaNative.ClearError_ns(error);
            }

            throw new InvalidOperationException($"Falha ao abrir o arquivo: {path}. Detalhe: {message}");
        }

        try
        {
            var net = NeticaNative.ReadNet_bn(stream, 0);
            if (net == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Falha ao ler a rede: {path}");
            }

            NeticaNative.CompileNet_bn(net);
            return new NeticaNetwork(net);
        }
        finally
        {
            NeticaNative.DeleteStream_ns(stream);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        var mesg = new StringBuilder(MesgLen);
        _ = NeticaNative.CloseNetica_bn(_env, mesg);

        _env = IntPtr.Zero;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(NeticaEnvironment));
        }
    }
}

public sealed class NeticaNetwork : IDisposable
{
    private IntPtr _net;
    private bool _disposed;

    internal NeticaNetwork(IntPtr net)
    {
        _net = net;
    }

    public void SetFinding(string nodeName, string stateName)
    {
        ThrowIfDisposed();

        var node = NeticaNative.GetNodeNamed_bn(nodeName, _net);
        if (node == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Nó não encontrado: {nodeName}");
        }

        var state = NeticaNative.GetStateNamed_bn(stateName, node);
        if (state < 0)
        {
            throw new InvalidOperationException($"Estado não encontrado: {stateName}");
        }

        NeticaNative.EnterFinding_bn(node, state);
    }

    public void RetractFindings()
    {
        ThrowIfDisposed();
        NeticaNative.RetractNetFindings_bn(_net);
    }

    public IReadOnlyDictionary<string, float> GetBeliefs(string nodeName)
    {
        ThrowIfDisposed();

        var node = NeticaNative.GetNodeNamed_bn(nodeName, _net);
        if (node == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Nó não encontrado: {nodeName}");
        }

        var numStates = NeticaNative.GetNodeNumberStates_bn(node);
        var probsPtr = NeticaNative.GetNodeBeliefs_bn(node);

        var probs = new float[numStates];
        Marshal.Copy(probsPtr, probs, 0, numStates);

        var result = new Dictionary<string, float>(numStates, StringComparer.Ordinal);
        for (var i = 0; i < numStates; i++)
        {
            var namePtr = NeticaNative.GetNodeStateName_bn(node, i);
            var name = Marshal.PtrToStringAnsi(namePtr) ?? i.ToString();
            result[name] = probs[i];
        }

        return result;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        NeticaNative.DeleteNet_bn(_net);
        _net = IntPtr.Zero;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(NeticaNetwork));
        }
    }
}
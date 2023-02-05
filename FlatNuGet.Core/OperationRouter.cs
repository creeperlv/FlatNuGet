using LibCLCC.NET.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatNuGet.Core
{
    public static class OperationRouter
    {
        public static ChainAction<Operation> actions = new ChainAction<Operation>();
        public static void BroadcastOperation(Operation operation)
        {
            Task.Run(() => actions.Invoke(operation));
        }
    }
    [Serializable]
    public class Operation
    {
        public OperationType OperationType;
        public object Message;
    }
    [Serializable]
    public class CombinedData
    {
        public object L;
        public object R;
    }
    public enum OperationType
    {
        Common,Download,Extract,Error,Warning,CacheHit
    }
}

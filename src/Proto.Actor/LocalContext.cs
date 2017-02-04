using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Proto.Mailbox;

namespace Proto
{
    public class Context : IMessageInvoker, IContext, ISupervisor
    {
        private readonly Stack<Receive> _behavior;
        private HashSet<PID> _children;
        private object _message;
        private bool _restarting;
        private Stack<object> _stash;
        private bool _stopping;
        private HashSet<PID> _watchers;
        private HashSet<PID> _watching;
        private RestartStatistics _restartStatistics;
        private Timer _receiveTimeoutTimer;
        private Receive _receive;
        private readonly Func<IActor> _producer;
        private readonly ISupervisorStrategy _supervisorStrategy;
        private readonly Receive _middleware;


        public Context(Func<IActor> producer, ISupervisorStrategy supervisorStrategy, Receive middleware, PID parent)
        {
            _producer = producer;
            _supervisorStrategy = supervisorStrategy;
            _middleware = middleware;
            Parent = parent;
            _behavior = new Stack<Receive>();
            _behavior.Push(ActorReceive);

            IncarnateActor();
        }
        
        public IReadOnlyCollection<PID> Children => _children.ToList();
        public IActor Actor { get; private set; }
        public PID Parent { get; }
        public PID Self { get; internal set; }

        public object Message
        {
            get
            {
                var r = _message as MessageSender;
                return r != null ? r.Message : _message;
            }
            private set { _message = value; }
        }

        public PID Sender => (_message as MessageSender)?.Sender;
        public TimeSpan ReceiveTimeout { get; private set; }


        public void Stash()
        {
            if (_stash == null)
            {
                _stash = new Stack<object>();
            }
            _stash.Push(Message);
        }

        internal static Task DefaultReceive(IContext context)
        {
            var c = (Context) context;
            if (c.Message is PoisonPill)
            {
                c.Self.Stop();
                return Proto.Actor.Done;
            }
            else
            {
                return c._receive(context);
            }
        }

        public void Respond(object message)
        {
            Sender.Tell(message);
        }

        public PID Spawn(Props props)
        {
            var id = ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, id);
        }

        public PID SpawnPrefix(Props props, string prefix)
        {
            var name = prefix + ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, name);
        }

        public PID SpawnNamed(Props props, string name)
        {
            var pid = props.Spawn($"{Self.Id}/{name}", Self);
            if (_children == null)
            {
                _children = new HashSet<PID>();
            }
            _children.Add(pid);
            Watch(pid);
            return pid;
        }

        public void SetBehavior(Receive receive)
        {
            _behavior.Clear();
            _behavior.Push(receive);
            _receive = receive;
        }

        public void PushBehavior(Receive receive)
        {
            _behavior.Push(receive);
            _receive = receive;
        }

        public void PopBehavior()
        {
            if (_behavior.Count <= 1)
            {
                throw new Exception("Can not unbecome actor base behaviour");
            }
            _receive=_behavior.Pop();
        }

        public void Watch(PID pid)
        {
            pid.SendSystemMessage(new Watch(Self));
            if(_watching == null) _watching = new HashSet<PID>();
            _watching.Add(pid);
        }

        public void Unwatch(PID pid)
        {
            pid.SendSystemMessage(new Unwatch(Self));
            if (_watching == null) _watching = new HashSet<PID>();
            _watching.Remove(pid);
        }

        public async Task InvokeSystemMessageAsync(object msg)
        {
            try
            {
                switch (msg)
                {
                    case Started s:
                        await InvokeUserMessageAsync(s);
                        break;
                    case Stop _:
                        await HandleStopAsync();
                        break;
                    case Terminated t:
                        await HandleTerminatedAsync(t);
                        break;
                    case Watch w:
                        HandleWatch(w);
                        break;
                    case Unwatch uw:
                        HandleUnwatch(uw);
                        break;
                    case Failure f:
                        HandleFailure(f);
                        break;
                    case Restart r:
                        await HandleRestartAsync();
                        break;
                    case SuspendMailbox sm:
                        break;
                    case ResumeMailbox rm:
                        break;
                    default:
                        Console.WriteLine("Unknown system message {0}", msg);
                        break;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Error handling SystemMessage {0}", x);
            }
        }

        public async Task InvokeUserMessageAsync(object msg)
        {
            //var influenceTimeout = true;
            //if (ReceiveTimeout > TimeSpan.Zero)
            //{
            //    var notInfluenceTimeout = msg is INotInfluenceReceiveTimeout;
            //    influenceTimeout = !notInfluenceTimeout;
            //    if (influenceTimeout)
            //        StopReceiveTimeout();
            //}

            await ProcessMessageAsync(msg);

            //if (ReceiveTimeout > TimeSpan.Zero && influenceTimeout)
            //    ResetReceiveTimeout();
        }

        private async Task ProcessMessageAsync(object msg)
        {

            Message = msg;
            if (_middleware != null)
            {
                await _middleware(this);
            }
            else
            {
                await DefaultReceive(this);
            }

        }

        public void EscalateFailure(PID who, Exception reason)
        {
            Self.SendSystemMessage(SuspendMailbox.Instance);
            Parent.SendSystemMessage(new Failure(who, reason, _restartStatistics));
        }

        private void IncarnateActor()
        {
            _restarting = false;
            _stopping = false;
            Actor = _producer();
            SetBehavior(ActorReceive);
        }

        private async Task HandleRestartAsync()
        {
            _stopping = false;
            _restarting = true;

            await InvokeUserMessageAsync(new Restarting());
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Stop();
                }
            }
            await TryRestartOrTerminateAsync();
        }

        private void HandleUnwatch(Unwatch uw)
        {
            _watchers?.Remove(uw.Watcher);
        }

        private void HandleWatch(Watch w)
        {
            if (_watchers == null)
            {
                _watchers = new HashSet<PID>();
            }
            _watchers.Add(w.Watcher);
        }

        private void HandleFailure(Failure msg)
        {
            if (Actor is ISupervisorStrategy supervisor)
            {
                supervisor.HandleFailure(this, msg.Who, msg.RestartStatistics, msg.Reason);
                return;
            }
            _supervisorStrategy.HandleFailure(this, msg.Who, msg.RestartStatistics, msg.Reason);
        }

        private async Task HandleTerminatedAsync(Terminated msg)
        {
            _children.Remove(msg.Who);
            _watching.Remove(msg.Who);
            await InvokeUserMessageAsync(msg);
            await TryRestartOrTerminateAsync();
        }

        private void HandleRootFailure(Failure failure)
        {
            Supervision.DefaultStrategy.HandleFailure(this, failure.Who, failure.RestartStatistics, failure.Reason);
        }

        private async Task HandleStopAsync()
        {
            _restarting = false;
            _stopping = true;
            //this is intentional
            await InvokeUserMessageAsync(Stopping.Instance);
            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Stop();
                }
            }
            await TryRestartOrTerminateAsync();
        }

        private async Task TryRestartOrTerminateAsync()
        {
            if (_receiveTimeoutTimer != null)
            {
                StopReceiveTimeout();
                _receiveTimeoutTimer = null;
                ReceiveTimeout = TimeSpan.Zero;
            }

            if (_children?.Count > 0)
            {
                return;
            }

            if (_restarting)
            {
                await RestartAsync();
                return;
            }

            if (_stopping)
            {
                await StopAsync();
            }
        }

        private async Task StopAsync()
        {
            ProcessRegistry.Instance.Remove(Self);
            //This is intentional
            await InvokeUserMessageAsync(Stopped.Instance);
            //Notify watchers
        }

        private async Task RestartAsync()
        {
            IncarnateActor();
            Self.SendSystemMessage(ResumeMailbox.Instance);

            await InvokeUserMessageAsync(Started.Instance);
            if (_stash != null)
            {
                while (_stash.Any())
                {
                    var msg = _stash.Pop();
                    await InvokeUserMessageAsync(msg);
                }
            }
        }

        private Task ActorReceive(IContext ctx)
        {
            return Actor.ReceiveAsync(ctx);
        }

        public void SetReceiveTimeout(TimeSpan duration)
        {
            if (duration == ReceiveTimeout)
                return;
            if (duration > TimeSpan.Zero)
                StopReceiveTimeout();
            if (duration < TimeSpan.FromMilliseconds(1))
                duration = TimeSpan.FromMilliseconds(1);
            ReceiveTimeout = duration;
            if (ReceiveTimeout > TimeSpan.Zero)
            {
                if (_receiveTimeoutTimer == null)
                {
                    _receiveTimeoutTimer = new Timer(ReceiveTimeoutCallback, null, ReceiveTimeout, ReceiveTimeout);
                }
                else
                {
                    ResetReceiveTimeout();
                }
            }
        }

        private void ResetReceiveTimeout()
        {
            _receiveTimeoutTimer?.Change(ReceiveTimeout, ReceiveTimeout);
        }

        private void StopReceiveTimeout()
        {
            _receiveTimeoutTimer?.Change(-1, -1);
        }

        private void ReceiveTimeoutCallback(object state)
        {
            Self.Request(Proto.ReceiveTimeout.Instance, null);
        }

        public void EscalateFailure(Exception reason, object message)
        {
            if (_restartStatistics == null)
            {
                _restartStatistics = new RestartStatistics(1, null);
            }
            var failure = new Failure(Self, reason, _restartStatistics);
            if (Parent == null)
            {
                HandleRootFailure(failure);
            }
            else
            {
                Self.SendSystemMessage(SuspendMailbox.Instance);
                Parent.SendSystemMessage(failure);
            }
        }

        public Task ReceiveAsync(object message)
        {
            return ProcessMessageAsync(message);
        }
    }
}
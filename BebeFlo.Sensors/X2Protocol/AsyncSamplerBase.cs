using System;
using System.Collections.Generic;
using System.ComponentModel;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.X2Protocol
{
	public abstract class AsyncSamplerBase<RecordT> : ExBackgroundWorker
	{
		public bool AbortedDueToTransitTimeError
		{
			get
			{
				return this._abortReason.HasValue && this._abortReason.Value == X2StreamStatus.TransitTimeError;
			}
		}
        //参数1、采样函数；2、数据处理函数；3、错误处理函数
		protected AsyncSamplerBase(Action<Func<IList<RecordT>, bool>> samplingFunc, Func<IList<RecordT>, bool> dataCb, ExBackgroundWorker.AsyncErrorOccurredHandler errorCb) : base(new ExBackgroundWorker.AsyncErrorOccurredHandler(AsyncSamplerBase<RecordT>.InternalErrorCb))
		{
			if (samplingFunc == null)
			{
				throw new ArgumentNullException("samplingFunc");
			}
			if (dataCb == null)
			{
				throw new ArgumentNullException("dataCb");
			}
			if (errorCb == null)
			{
				throw new ArgumentNullException("errorCb");
			}
			this._samplingFunc = samplingFunc;//采样函数
			this._dataCb = dataCb;//数据处理函数
			this._errorCb = errorCb;//错误处理函数
			base.PropertyChanged += delegate(object s, PropertyChangedEventArgs ea)
			{
				if (ea.PropertyName == "IsRunning" && base.IsRunning)
				{
					this._abortReason = null;
				}
			};
		}

		protected override void DoWork()
		{
			this._samplingFunc(delegate(IList<RecordT> vals)
			{
				bool result;
				lock (base.SyncRoot)
				{
					bool flag3 = !base.CancellationPending;
					if (flag3)
					{
						if (this._buffer == null)
						{
							this._buffer = new List<RecordT>(vals);
						}
						else
						{
							this._buffer.AddRange(vals);
						}
						base.FeedbackAsync(new Action(this.FlushBuffer));
					}
					result = flag3;
				}
				return result;
			});
			lock (base.SyncRoot)
			{
				this._buffer = null;
			}
		}

		private static void InternalErrorCb(ExBackgroundWorker sender, Exception error)
		{
			AsyncSamplerBase<RecordT> asyncSamplerBase = (AsyncSamplerBase<RecordT>)sender;
			Exception ex = error;
			X2ProtocolException ex2 = null;
			while (ex2 == null && ex != null)
			{
				ex2 = (ex as X2ProtocolException);
				ex = ex.InnerException;
			}
			asyncSamplerBase._abortReason = ((ex2 != null) ? new X2StreamStatus?(ex2.ErrorStatus) : null);
			asyncSamplerBase._errorCb(sender, error);
		}

		private void FlushBuffer()
		{
			IList<RecordT> list = null;
			lock (base.SyncRoot)
			{
				list = this._buffer;
				this._buffer = null;
				if (!base.IsRunning || base.CancellationPending)
				{
					return;
				}
			}
			bool flag2 = true;
			if (list != null && list.Count > 0)
			{
				flag2 &= this._dataCb(list);
			}
			if (!flag2)
			{
				base.StopAsync();
			}
		}

		private readonly Action<Func<IList<RecordT>, bool>> _samplingFunc;

		private readonly Func<IList<RecordT>, bool> _dataCb;

		private readonly ExBackgroundWorker.AsyncErrorOccurredHandler _errorCb;

		private List<RecordT> _buffer;

		private X2StreamStatus? _abortReason;
	}
}
